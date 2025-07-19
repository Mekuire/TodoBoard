using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    public class Scaleable2D : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _uiElement;
        
        private RectTransform _canvasRect;
        
        public event Action OnDragEnded;
        
        private void Start()
        {
            _uiElement = _uiElement != null ? _uiElement : GetComponent<RectTransform>();
            
            _canvasRect = _uiElement.root as RectTransform;
        }
        
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
            Scale(_uiElement.sizeDelta + delta);
        }

        private void Scale(Vector2 newScale)
        {
            // Переместить
            _uiElement.sizeDelta = newScale;

            // Получаем размеры
            Vector2 size = _uiElement.sizeDelta;
            Vector2 pos = _uiElement.anchoredPosition;
            Vector2 canvasSize = _canvasRect.sizeDelta;

            // Границы
            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;
            float canvasHalfWidth = canvasSize.x * 0.5f;
            float canvasHalfHeight = canvasSize.y * 0.5f;

            // Ограничиваем по X
            pos.x = Mathf.Clamp(pos.x, -canvasHalfWidth + halfWidth, canvasHalfWidth - halfWidth);

            // Ограничиваем по Y
            pos.y = Mathf.Clamp(pos.y, -canvasHalfHeight + halfHeight, canvasHalfHeight - halfHeight);

            // Применяем
            _uiElement.anchoredPosition = pos;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnded?.Invoke();
        }
    }
}
