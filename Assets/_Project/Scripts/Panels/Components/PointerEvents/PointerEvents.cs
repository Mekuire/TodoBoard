using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    public class PointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action OnPointerEntered;
        public event Action OnPointerExited;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("OnPointerEnter");
            OnPointerEntered?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("OnPointerExit");
            OnPointerExited?.Invoke();   
        }
    }
}
