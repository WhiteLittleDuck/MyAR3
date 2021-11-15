using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NRKernal.NRExamples
{
    /// <summary> A user define button. </summary>
    public class ConfirmButton : MonoBehaviour, IPointerClickHandler
    {
        public Action<string> OnClick;

        public void OnPointerClick(PointerEventData eventData){
            UpperController.instance.confirm();
        }
    }
}