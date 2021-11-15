using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TerminalsController : MonoBehaviour {
    public GameObject textt;
    public Tube[] tubes;
    public static TerminalsController instance;
    void Start(){}
    void Update(){}

    void Awake(){
        instance=this;
        tubes = new Tube[transform.childCount];
        for(int i =0;i<transform.childCount;i++){
        GameObject g = transform.GetChild(i).gameObject;
            if(g.GetComponent<Tube>()==null){
                g.AddComponent<Tube>().parent = this;
            }
            tubes[i]=g.GetComponent<Tube>();
        }
    }

    public void setColor(int[] data){
        int[][] result = new int[6][];//前三个是left，后三个是right
        int ptr=0;

        for(int i=0;i<6;i++){
            result[i]=new int[6];
            for(int j=0;j<2;j++){
                result[i][j]=data[ptr++];
            }
        }

        for(int i=0;i<6;i++){
            for(int j=2;j<4;j++){
                result[i][j]=data[ptr++];
            }
        }

        for(int i=0;i<6;i++){
            tubes[i].setTerminal(result[i]);
        }


    }
    public void setHighlight(int[] data){
        string s="";
        int[][] result=new int[6][];
        int ptr=0;
        for(int i=0;i<6;i++){
            result[i]=new int[2];
            s+=data[ptr];
            result[i][0]=data[ptr++];
        } 
        for(int i=0;i<6;i++){
            s+=data[ptr];
            result[i][1]=data[ptr++];
        }
        
        for (int i=0;i<6;i++){
            tubes[i].setHighlight(result[i]);
        }
        // textt.GetComponent<Text>().text=s;
    }
}
       
public class Tube : MonoBehaviour {
    public TerminalsController parent;
    public Color myColor;
    // self.colours = {'red': 0, 'purple': 1, 'green': 2, 'orange': 3, 'blue': 4}
    // private Color[] colors = new Color[]{Color.red,Color.blue,Color.yellow,Color.green,Color.white};
    private Material[] terminalColor=new Material[5];
    private GameObject leftTerminal;
    private GameObject rightTerminal;
    private Material defaultMaterial;
    private Material highlightMaterial;

    void Start(){}

    void Awake(){
        leftTerminal=transform.GetChild(0).gameObject;
        rightTerminal=transform.GetChild(1).gameObject;
        myColor=new Color(133,48,41);
        defaultMaterial=Resources.Load<Material>("Materials/2");
        highlightMaterial = Resources.Load<Material>("Materials/3");
        terminalColor[0]=Resources.Load<Material>("Materials/red");
        terminalColor[1]=Resources.Load<Material>("Materials/purple");
        terminalColor[2]=Resources.Load<Material>("Materials/green");
        terminalColor[3]=Resources.Load<Material>("Materials/orange");
        terminalColor[4]=Resources.Load<Material>("Materials/blue");

    }

    public void setTerminal(int[] data){
        leftTerminal.transform.GetChild(0).gameObject.transform.GetComponent<MeshRenderer>().material = terminalColor[data[0]];
        leftTerminal.transform.GetChild(1).gameObject.transform.GetComponent<MeshRenderer>().material = terminalColor[data[1]];
        // leftTerminal.transform.GetChild(2).gameObject.transform.GetComponent<MeshRenderer>().material.color = colors[data[2]];
        rightTerminal.transform.GetChild(0).gameObject.transform.GetComponent<MeshRenderer>().material = terminalColor[data[2]];
        rightTerminal.transform.GetChild(1).gameObject.transform.GetComponent<MeshRenderer>().material = terminalColor[data[3]];
        // rightTerminal.transform.GetChild(2).gameObject.transform.GetComponent<MeshRenderer>().material.color = colors[data[5]];

        leftTerminal.gameObject.GetComponent<MeshRenderer>().material=defaultMaterial;
        rightTerminal.gameObject.GetComponent<MeshRenderer>().material=defaultMaterial;
    }

    public void setHighlight(int[] data){
        if(data[0]==1){
            leftTerminal.gameObject.GetComponent<MeshRenderer>().material=highlightMaterial;
        }else{
            // leftTerminal.gameObject.GetComponent<MeshRenderer>().material=defaultMaterial;
        }
        if(data[1]==1){
            rightTerminal.gameObject.GetComponent<MeshRenderer>().material=highlightMaterial;
        }else{
            // rightTerminal.gameObject.GetComponent<MeshRenderer>().material=defaultMaterial;
        }
    }
}
// }
