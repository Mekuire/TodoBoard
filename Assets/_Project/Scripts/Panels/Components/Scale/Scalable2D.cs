using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    public class Scalable2D : MovableWindow, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Vector2 minSize = new Vector2(100, 100);
        [SerializeField] private Vector2 maxSize = new Vector2(1920, 1080);
        [SerializeField] private Vector2 _pivot = new Vector2(0f, 1f);

        private Vector2 _startMousePos;
        private Vector2 _startSize;
        
        public event Action OnDragEnded;

        protected override void Awake()
        {
            base.Awake();
            
            _uiElement.pivot = _pivot;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _uiElement,
                eventData.position,
                eventData.pressEventCamera,
                out var currentMousePos
            );

            Vector2 diff = currentMousePos - _startMousePos;
            diff.y = -diff.y;
            Scale(diff);
            
            PreventFromMoveOutside();
        }

        private void Scale(Vector2 delta)
        {
            Vector2 newSize = _startSize + delta;
            newSize.x = Mathf.Clamp(newSize.x, minSize.x, maxSize.x);
            newSize.y = Mathf.Clamp(newSize.y, minSize.y, maxSize.y);

            _uiElement.sizeDelta = newSize;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _uiElement,
                eventData.position,
                eventData.pressEventCamera,
                out _startMousePos
            );
            
            _startSize = _uiElement.sizeDelta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnded?.Invoke();
        }
    }
}
