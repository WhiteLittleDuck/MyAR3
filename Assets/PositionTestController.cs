using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionTestController : MonoBehaviour
{
    Text myText;
    public GameObject camera;
    public GameObject leftDetector;
    public GameObject rightDetector;
    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        var pos = camera.transform.position.x;
        var leftPos=leftDetector.transform.position.x;
        var rightPos=rightDetector.transform.position.x;
        string show="";
        if(pos<leftPos){
            show+="left can see\n";
        }else{
            show+="left cannot see\n";
        }
        if(pos>rightPos){
            show+="right can see";
        }else{
            show+="right cannot see\n";
        }
        myText.text=show;
        // myText.text = "x:"+pos.x+"\ny:"+pos.y+"\nz:"+pos.z;
    }
}
