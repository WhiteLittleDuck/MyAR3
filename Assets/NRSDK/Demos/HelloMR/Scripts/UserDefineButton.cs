
// using System;
// using UnityEngine;
// using UnityEngine.EventSystems;

// namespace NRKernal.NRExamples
// {
//     /// <summary> A user define button. </summary>
//     public class UserDefineButton : MonoBehaviour, IPointerClickHandler
//     {
//         public Action<string> OnClick;
//         public GameObject camera;
//         public GameObject leftDetector;
//         public GameObject rightDetector;

//         public void OnPointerClick(PointerEventData eventData){
//             var pos = camera.transform.position.x;
//             var leftPos=leftDetector.transform.position.x;
//             var rightPos=rightDetector.transform.position.x;
//             string send = "";
//             if(pos<leftPos){
//                 send+="1 ";//left can see
//             }else{
//                 send+="0 ";//left cannot see
//             }
        
//             if(pos>rightPos){//right can see
//                 send+="1";
//             }else{//right cannot see
//                 send+="0";
//             }
//             TestSocket.instance.sendHelpRequest(send);
//         }
//     }
// }
/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NRKernal.NRExamples
{
    /// <summary> A user define button. </summary>
    public class UserDefineButton : MonoBehaviour, IPointerClickHandler
    {
        /// <summary> The on click. </summary>
        public Action<string> OnClick;

        public void OnPointerClick(PointerEventData eventData){
            // CountDown.instance.TotalTime=300;
            // TestSocket.instance.sendReady();
        }
    }
}