using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    [RequireComponent(typeof(RectTransform))]
    public class Draggable2D : MonoBehaviour, IDragHandler
    {
        [SerializeField] private RectTransform _uiElement;
        
        private RectTransform _canvasRect;
        
        private void Start()
        {
            _uiElement = _uiElement != null ? _uiElement : GetComponent<RectTransform>();
            
            // Centering Anchor
            Vector2 savedPosition = _uiElement.position;
            _uiElement.anchorMin = new Vector2(0.5f, 0.5f);
            _uiElement.anchorMax = new Vector2(0.5f, 0.5f);
            _uiElement.pivot = new Vector2(0.5f, 0.5f);
            _uiElement.position = savedPosition;
            
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
            Move(_uiElement.anchoredPosition + delta);
        }

        private void Move(Vector2 newPosition)
        {
            // Переместить
            _uiElement.anchoredPosition = newPosition;

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
    }
}