using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class UpperController : MonoBehaviour{
    public GameObject textt;
    public Button[] buttons;
    public int select;
    public GameObject textToSpeech;    // Start is called before the first frame update
    public static UpperController instance;

    public bool confirmming=false;    
    void Start(){}

    // Update is called once per frame
    void Update(){}

    void Awake(){
        instance=this;
        select = -1;
        buttons = new Button[transform.childCount];
        for(int i =0;i<transform.childCount;i++){
            GameObject g = transform.GetChild(i).gameObject;
            if(g.GetComponent<Button>()==null){
                g.AddComponent<Button>().parent = this;
            }
            buttons[i]=g.GetComponent<Button>();
            buttons[i].index=i;
        }
    }
    public void OnChildClick(){}
    public void setColor(int[] data){
        int ptr=0;
        foreach (Button b in buttons){
            b.setButton(data[ptr++]);
        }
    }
    public void setHightlight(int index){
        if(index>=0&&index<=5){
            buttons[index].highlight();
        }
    }
    
    public void selectBtn(int index){
        if(select>=0){
            buttons[select].cancelSelect();
        }
        select=index;
        Debug.Log(select);
    }

    public void confirm(){
        Debug.Log("!"+select);
        if(select<0){
            Debug.Log("Must Select Something!");
            return;
        }
        if(TestSocket.instance.tutorialMode){
            TestSocket.instance.tutorialConfirm(select);
            buttons[select].cancelSelect();
            select=-2;

        }else{
            confirmming=true;
            TestSocket.instance.sendHumanControl(select);
        }
    }
    public void endConfirm(){
        if(select>=0){
            buttons[select].cancelSelect();
        }
        select=-2;
        confirmming=false;
    }
}
public class Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler{
    public UpperController parent;
    private Color[] colors = new Color[]{Color.black,Color.white};
    public int index;
    public int status=0;//0:blocked, 1:selectable, 2:selected
    public Color myColor=Color.gray;
    
    void Start(){
        // myColor=transform.GetComponent<MeshRenderer>().material.color;
    }
    
    public void setButton(int i){
        if(i==0) i=1;
        else i=0;
        transform.GetChild(0).gameObject.transform.GetComponent<MeshRenderer>().material.color = colors[i];
        status=i;
    }

    public void OnPointerClick(PointerEventData eventData){
        if(status==1){
            status=2;
            transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
            myColor=Color.yellow;
            parent.selectBtn(index);
            // TestSocket.instance.sendHumanControl(index);
            // transform.GetComponent<MeshRenderer>().material.color = myColor;
        }
        // parent.textt.GetComponent<Text>().text=index+":"+status+"";
    }
              
    public void OnPointerEnter(PointerEventData eventData){
        // Debug.Log("hover:"+parent.select);
        if(status==1){
            transform.GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(status<=1){
            transform.GetComponent<MeshRenderer>().material.color = myColor;
        }
    }

    public void cancelSelect(){
        status=1;
        transform.GetComponent<MeshRenderer>().material.color = Color.gray;
        myColor=Color.gray;
    }

    public void highlight(){
       transform.GetChild(0).gameObject.transform.GetComponent<MeshRenderer>().material.color=Color.red;
    }
}
