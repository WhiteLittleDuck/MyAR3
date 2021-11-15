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
    public class startButton : MonoBehaviour, IPointerClickHandler
    {
        /// <summary> The on click. </summary>
        public Action<string> OnClick;

        public void OnPointerClick(PointerEventData eventData){
            // CountDown.instance.TotalTime=120;
            // // TextToSpeechController.instance.audioSource.Pause();
            TestSocket.instance.startTutorial();
        }
    }
}