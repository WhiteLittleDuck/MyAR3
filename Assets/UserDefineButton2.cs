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
    public class UserDefineButton2 : MonoBehaviour, IPointerClickHandler
    {
        /// <summary> The on click. </summary>
        public Action<string> OnClick;
        public GameObject box;
        // float y = -0.5f;

        /// <summary> <para>Use this callback to detect clicks.</para> </summary>
        /// <param name="eventData"> Current event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // var temp =  box.transform.rotation;//new Vector3(X, Y, Z)
            // temp.y=(temp.y+45)%360;
            // y-=0.05f;
            // if(y<=-0.8f) y+=0.2f;
            box.transform.position=new Vector3(box.transform.position.x, box.transform.position.y-0.05f, box.transform.position.z);
            // if (OnClick != null)
            // {
            //     OnClick(gameObject.name);
            // }
        }
    }
}