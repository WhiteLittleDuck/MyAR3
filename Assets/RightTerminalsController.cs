using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// namespace NRKernal.NRExamples
// {
       public class RightTerminalsController : MonoBehaviour
       {
              // private Transform[] children;
              // Start is called before the first frame update
              public ChildObj2[] terminals;
              void Start(){}

              // Update is called once per frame
              void Update(){}

              void Awake()
              {
                     terminals = new ChildObj2[transform.childCount];
                     for(int i =0;i<transform.childCount;i++){
                            GameObject g = transform.GetChild(i).gameObject;
                            if(g.GetComponent<ChildObj2>()==null){
                                   g.AddComponent<ChildObj2>().parent = this;
                            }
                            terminals[i]=g.GetComponent<ChildObj2>();
                     }
              }
              public void OnChildClick()
              {
                     //Do what ever
              }
              public void setColor(int[] data){
                     int ptr=0;
                     foreach (ChildObj2 t in terminals)
                     {
                            t.setTerminal(data[ptr++],data[ptr++],data[ptr++]);
                     }
              }
              public void setHightlight(int[] data){
                     for (int i=0;i<terminals.Length;i++){
                         terminals[i].setHightlight(data[i]);
                     }
              }
       }
       public class ChildObj2 : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
       {
              public RightTerminalsController parent;
              public Color myColor=Color.white;
              private Color[] colors = new Color[]{Color.red,Color.black,Color.blue};
              void Start(){}

              public void setTerminal(int middle, int upper, int lower){
                     transform.GetChild(0).gameObject.transform.GetComponent<MeshRenderer>().material.color = colors[middle];
                     transform.GetChild(1).gameObject.transform.GetComponent<MeshRenderer>().material.color = colors[upper];
                     transform.GetChild(2).gameObject.transform.GetComponent<MeshRenderer>().material.color = colors[lower];

              }

              public void OnPointerClick(PointerEventData eventData){}

              public void setHightlight(int i){
                     if(i==1){
                            transform.GetComponent<MeshRenderer>().material.color=Color.red;
                            myColor=Color.red;
                     }
              }
              
              public void OnPointerEnter(PointerEventData eventData)
              {
                     // transform.GetComponent<MeshRenderer>().material.color = Color.green;
              }
              // public void OnPointerClick(PointerEventData eventData){}

              public void OnPointerExit(PointerEventData eventData)
              {
                     // transform.GetComponent<MeshRenderer>().material.color =myColor;
              }
       }
// }
