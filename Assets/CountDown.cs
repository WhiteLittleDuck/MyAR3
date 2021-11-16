using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public static CountDown instance;
    public GameObject camera;
    public GameObject leftDetector;
    public GameObject rightDetector;

    // Update is called once per frame
    void Update(){}
    public GameObject text;
    public int TimeSetting;
    private int TotalTime;
    void Awake(){
        instance=this;
    }

    void Start (){
        TotalTime=TimeSetting;
        StartCoroutine(Countdown());
    }
    public void resetTimer(){
        TotalTime=TimeSetting;
    }

    IEnumerator Countdown(){
        while (true){
            if(TestSocket.instance.mode!=2){
            //     TestSocket.instance.tutorialHelp(); automatically request for help in each second in tutorial mode
            }else
            if(TotalTime >=0){
                int minute = TotalTime/60;
                int sec = TotalTime%60;
                text.GetComponent<Text>().text = attachZero(minute)+":"+attachZero(sec);
                TotalTime--;

                // automatic send position information for each second
                // if(TestSocket.instance.tutorialCode==99){
                //     var pos = camera.transform.position.x;
                //     var leftPos=leftDetector.transform.position.x;
                //     var rightPos=rightDetector.transform.position.x;
                //     string send = "";
                //     if(pos<leftPos){
                //         send+="1 ";//left can see
                //     }else{
                //         send+="0 ";//left cannot see
                //     }
        
                //     if(pos>rightPos){//right can see
                //         send+="1";
                //     }else{//right cannot see
                //         send+="0";
                //     }
                
                //     TestSocket.instance.sendHelpRequest(send);
                // }

            }else if(TotalTime==-1){
                TestSocket.instance.timeout();
                TotalTime--;
            }

            yield return new WaitForSeconds(1);
        }
    }

    public string attachZero(int num){
        if(num<10){
            return "0"+num.ToString();
        }
        return num.ToString();
    }
}
