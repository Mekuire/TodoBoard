using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    [RequireComponent(typeof(RectTransform))]
    public class Draggable2D : MovableWindow, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, 
                eventData.position, 
                eventData.pressEventCamera, 
                out var localPoint
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, 
                eventData.position - eventData.delta, 
                eventData.pressEventCamera, 
                out var offset
            );

            Vector2 delta = localPoint - offset;
            Move(_uiElement.anchoredPosition + delta);
        }

        private void Move(Vector2 newPosition)
        {
            _uiElement.anchoredPosition = newPosition;

            PreventFromMoveOutside();
        }
    }
}