using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    [RequireComponent(typeof(RectTransform))]
    public class Draggable2D : MonoBehaviour, IDragHandler
    {
        [SerializeField] private RectTransform _uiElement;
        
        private RectTransform _canvasRect;
        
        private void Awake()
        {
            _uiElement = _uiElement != null ? _uiElement : GetComponent<RectTransform>();
            
            // Centering Anchor
            Vector2 savedPosition = _uiElement.position;
            _uiElement.anchorMin = new Vector2(0.5f, 0.5f);
            _uiElement.anchorMax = new Vector2(0.5f, 0.5f);
            _uiElement.pivot = new Vector2(0f, 1f);
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
            _uiElement.anchoredPosition = newPosition;

            Vector2 size = _uiElement.sizeDelta;
            Vector2 pos = _uiElement.anchoredPosition;
            Vector2 canvasSize = _canvasRect.sizeDelta;

            float canvasHalfW = canvasSize.x * 0.5f;
            float canvasHalfH = canvasSize.y * 0.5f;

            // берём текущий pivot
            Vector2 p = _uiElement.pivot;

            // рассчитываем границы для anchoredPosition.x:
            // минимальная x: центр экрана минус половина ширины канвы плюс offset по pivot
            float minX = -canvasHalfW + size.x * p.x;
            // максимальная x: +половина ширины канвы минус ширина * (1 - pivot.x)
            float maxX =  canvasHalfW - size.x * (1 - p.x);

            // аналогично для y:
            // минимальная y: -половина высоты канвы + высота * p.y
            float minY = -canvasHalfH + size.y * p.y;
            // максимальная y: +половина высоты канвы - высота * (1 - p.y)
            float maxY =  canvasHalfH - size.y * (1 - p.y);

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            _uiElement.anchoredPosition = pos;
        }

    }
}