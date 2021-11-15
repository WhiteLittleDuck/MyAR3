using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
 
public class TestSocket : MonoBehaviour {
    public string addr="127.0.0.1";//172.17.122.195//172.27.124.138
    public int port=12346;
    private Socket mySocket;
    public static TestSocket instance; //This instance

    //text board and test text board
    public GameObject testText; //GameObject CanvasTest: test text board to report the bug
    public GameObject hintText; //GameObject SpeakersCanvas: test to speech indication text board in 
    
    //vertical board with help and confirm button
    public GameObject helpButton; //GameObject CanvasCommu>Panel>HelpBtn
    public GameObject comfirmButton; //GameObject CanvasCommu>Panel>ConfirmBtn
    
    //bomb
    public GameObject bomb; //GameObject DClockbomb_Aged: bomb model
    public GameObject upobj; //GameObject DClockbomb_Aged>UpperPanel: the horizontal button panel
    
    //Canvas indicator
    public GameObject canvasReady; //GameObject ReadyCanvas: when connected to the model, it turns from "Connecting" to "Are you ready?"
    public GameObject readyStatus; //GameObject ReadyPanel>CanvasIndicator>Panel>readyStatus: the notification to show ready (connecting/explode/defuse/are you ready?) status
    public GameObject readyButton; //GameObject ReadyCanvas>CanvasIndicator>Panel>readyButton

    public GameObject canvasTutorial; //GameObject TutorialCanvas: show up each turn to decide stay in tutorial or start
    public GameObject tutorialStatus; //GameObject TutorialPanel>CanvasIndicator>Panel>tutorialStatus: the notification to show tutorial status(explode/defuse)

 
	//buffers and size
    private byte[] dataBytes= new byte[4096]; //buffer
    private int[] sideData; //buffer of both sides of terminals' total component
    private int terminalCnt=6; //terminal number on single side
    private int terminalColorCnt=2; //number of color for each termianl
    
    private int[] buttonData; //buffer of buttons on the upper panel
    private int buttonCnt=6; //number of button 
    
    private int[] sideHightlight; //highlight buffer
    private int buttonHightlight=-1; //only one button highlight
    private string[] readContent=new string[2]; //left hint text and right hint text  
    
    //indicator of the type of receiving messages
    //-2: communication info; others(-1,0,1,2,3): terminal info
    //-1: explode
    //0: continue
    //1: diffuse
    //2: first time request for terminal info
    //3: time out and explode
    private int status;
    
    public bool tutorialMode; //indicate wheter the program are in tutorial
    // status in tutorial mode
    // 1: start; 
    public int tutorialCode=0; 

    //tutorial data
    //             s|    left side color    |    right side color   |    btn    |
    //             1|1 1|2 2|3 3|4 4|5 5|6 6|1 1|2 2|3 3|4 4|5 5|6 6|1 2 3 4 5 6
    string tdata0="2$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$1$0$0$0$0$0$0$0";
    string tdata1="0$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$2$1$0$0$0$0$0";
    string tdata2="0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$0$1$1$0$0$0$0";
    //              s| left h    | right h   |d| text     #  text   |
    //              1|1 2 3 4 5 6|1 2 3 4 5 6|1| 
    string cdata0="-2$1$1$0$0$0$1$1$0$0$0$0$0$0$right text#lefttext";
    string cdata1="-2$1$1$0$0$0$1$0$1$0$0$0$1$1$right text#lefttext";
    string cdata2="-2$1$1$0$0$0$1$1$1$0$0$0$2$2$right text#lefttext";

    void Start () {
        instance=this;

        sideData=new int[2*terminalCnt*terminalColorCnt]; //calculate the size
        buttonData=new int[buttonCnt];
        sideHightlight=new int[terminalCnt*2];
        buttonHightlight=-1;
        readContent=new string[2];
        tutorialMode=true;
        
        bomb.SetActive(false);
        canvasReady.SetActive(false);
        canvasTutorial.SetActive(true);
        setTerminalInfo(tdata0.Split(new char[]{'$'})); //load tutorial into scence
    }

    //invoke by human click "Start" in CanvasTutorial
    public void startTutorial(){
        bomb.SetActive(true);
        tutorialCode=0;
    }

    //invoke by human click "Help" in tutorial mode
    public void tutorialHelp(){
        if(tutorialCode==0){
            setComInfo(cdata0.Split(new char[]{'$'}));
        }else if(tutorialCode==1){
            setComInfo(cdata1.Split(new char[]{'$'}));
        }else{
            setComInfo(cdata2.Split(new char[]{'$'}));
        }
    }

    //invoke by human click "Confirm" in tutorial mode
    public void tutorialConfirm(int select){
        //wrong: explode, terminate this tutorial turn
        if(select!=tutorialCode) { 
            tutorialStatus.GetComponent<Text>().text="Tutorial: EXPLODED!!!";
            TextToSpeechController.instance.explode();
            
            bomb.SetActive(false);
            canvasTutorial.SetActive(true);
            hintText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            hintText.GetComponent<Text>().text="No Instruction";

            setTerminalInfo(tdata0.Split(new char[]{'$'})); 
            return;
        }

        //correct
        if(tutorialCode==0){
            setTerminalInfo(tdata1.Split(new char[]{'$'}));
        }else if(tutorialCode==1){
            setTerminalInfo(tdata2.Split(new char[]{'$'}));
        }else{ //defuse
            tutorialStatus.GetComponent<Text>().text="Tutorial: DEFUSED~";
            TextToSpeechController.instance.diffuse();

            bomb.SetActive(false);
            canvasTutorial.SetActive(true);
            hintText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            hintText.GetComponent<Text>().text="No Instruction";
            
            setTerminalInfo(tdata0.Split(new char[]{'$'}));
        }
        tutorialCode++;
    }

    //invoke when human click "Skip" in tutorial canvas
    public void startConnect(){
        //initiate socket
        mySocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        mySocket.ReceiveTimeout = 10;
        //connect with server
        mySocket.Connect(new IPEndPoint(IPAddress.Parse(addr), port));//172.27.124.138//172.17.123.47
        mySocket.Blocking=false;
        
        readyButton.SetActive(false);
        canvasReady.SetActive(true);
        StartCoroutine(recieveData());
        readyStatus.GetComponent<Text>().text="Connecting Server ......";

    }

    //timely check recieve data buffer
    IEnumerator recieveData(){
        while (true){
            while(mySocket==null||(mySocket != null && mySocket.Available < 1)){
                yield return new WaitForSeconds(2);
            }

            int count = mySocket.Receive(dataBytes);
            string result = Encoding.UTF8.GetString(dataBytes, 0, count);
            Debug.Log(result);

		    string[] stringData = result.Trim().Split(new char[]{'$'});
            status = int.Parse(stringData[0]);

            //communication info
            if(status == -2){
                setComInfo(stringData);                
            }else{
            //terminal info
                helpButton.GetComponent<UnityEngine.UI.Button>().interactable  = true;
                comfirmButton.GetComponent<UnityEngine.UI.Button>().interactable  = true;
            UpperController.instance.endConfirm();
            if(status==1){
                //defuse
                readyStatus.GetComponent<Text>().text="DEFUSED~";
                bomb.SetActive(false);
                canvasReady.SetActive(true);
                hintText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                hintText.GetComponent<Text>().text="No Instruction";
                TextToSpeechController.instance.diffuse();
            }
            if(status==-1){
                //explode
                readyStatus.GetComponent<Text>().text="EXPLODED!!!";
                bomb.SetActive(false);
                canvasReady.SetActive(true);
                hintText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                hintText.GetComponent<Text>().text="No Instruction";
                TextToSpeechController.instance.explode();
            }
            if(status==2){//the first time
                readyStatus.GetComponent<Text>().text="Are you ready?";
                readyButton.SetActive(true);
            }
            if(status==3){//time out explod
                readyButton.SetActive(true);
            }
            setTerminalInfo(stringData);
            }
            yield return new WaitForSeconds(1);
        }
    }

    //set buffer data into bomb termianl object in colors
    void setTerminalInfo(string[] stringData){
        int ptr=1;
        for (int i = 0; i < 2*terminalCnt*terminalColorCnt; i++){
            sideData[i]=int.Parse(stringData[ptr++]);
        }

        for(int i=0;i<buttonCnt;i++){
            buttonData[i]=int.Parse(stringData[ptr++]);
        }

        TerminalsController.instance.setColor(sideData);
        UpperController.instance.setColor(buttonData);
    }

    //load communication info(hightlight and sounds) into scence
    void setComInfo(string[] stringData){
        int ptr =1;
        for(int i =0;i<terminalCnt*2;i++){
            sideHightlight[i]=int.Parse(stringData[ptr++]);
        }

        buttonHightlight=int.Parse(stringData[ptr++]);

        TerminalsController.instance.setHighlight(sideHightlight);
        UpperController.instance.setHightlight(buttonHightlight);
        
        readContent =stringData[ptr++].Split(new char[]{'#'});
        // string leftContent=readContent.Split(new char[]{' '})
        string toRead=readContent[0]+". "+readContent[1];
        string toShow="";
        if(readContent[0]!=""){
            toShow+=readContent[0];
            if(readContent[1]!=""){
                toShow+="\n";
            }
        }
        if(readContent[1]!=""){
            toShow+=readContent[1];
        }

        TextToSpeechController.instance.readText(toRead);
        hintText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
        hintText.GetComponent<Text>().text=toShow;
    }
	
	// Update is called once per frame
	void Update (){}

    //send message to sever
    //send the button human selection
    public void sendHumanControl(int index){//"0 index"
        helpButton.GetComponent<UnityEngine.UI.Button>().interactable  = false;
        comfirmButton.GetComponent<UnityEngine.UI.Button>().interactable  = false;
        string sendData = "0 "+index;
        mySocket.Send(Encoding.UTF8.GetBytes(sendData));
    }

    //signify sever human is ready
    public void sendReady(){//"ready"
        string sendData = "ready";
        mySocket.Send(Encoding.UTF8.GetBytes(sendData));
        tutorialCode=99;
    }

    //signify sever human needs help
    public void sendHelpRequest(string info){//"1 [human current position]"
        mySocket.Send(Encoding.UTF8.GetBytes("1 "+info));
    }

    //signify sever bomb times out and explode
    public void timeout(){
        string sendData = "9";
        mySocket.Send(Encoding.UTF8.GetBytes(sendData));
        readyStatus.GetComponent<Text>().text="TIMEOUT\nEXPLODED!!!";
        readyButton.SetActive(false);

        bomb.SetActive(false);
        canvasReady.SetActive(true);
        hintText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        hintText.GetComponent<Text>().text="No Instruction";
        TextToSpeechController.instance.explode();
    }
}