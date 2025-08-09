using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    public class PointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action OnPointerEntered;
        public event Action OnPointerExited;
        public event Action<PointerEventData> OnPointerClicked;
        public event Action<PointerEventData> OnDragBegin;
        public event Action<PointerEventData> OnDragEnd;
        public event Action<PointerEventData> OnDraging;
        
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

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClicked?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnDragBegin?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDraging?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnd?.Invoke(eventData);
        }
    }
}
