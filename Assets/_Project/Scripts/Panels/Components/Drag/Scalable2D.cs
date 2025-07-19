using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    public class Scalable2D : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Vector2 minSize = new Vector2(100, 100);
        [SerializeField] private Vector2 maxSize = new Vector2(1920, 1080);
        
        private Vector2 _startMousePos;
        private Vector2 _startSize;
        private RectTransform _canvasRect;
        
        public event Action OnDragEnded;
        
        private void Awake()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            
            _canvasRect = _rectTransform.root as RectTransform;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var currentMousePos
            );

            Vector2 diff = currentMousePos - _startMousePos;
            diff.y = -diff.y;
            Vector2 newSize = _startSize + diff;
            newSize.x = Mathf.Clamp(newSize.x, minSize.x, maxSize.x);
            newSize.y = Mathf.Clamp(newSize.y, minSize.y, maxSize.y);

            _rectTransform.sizeDelta = newSize;
            
            Vector2 size = _rectTransform.sizeDelta;
            Vector2 pos = _rectTransform.anchoredPosition;
            Vector2 canvasSize = _canvasRect.sizeDelta;

            float canvasHalfW = canvasSize.x * 0.5f;
            float canvasHalfH = canvasSize.y * 0.5f;

            // берём текущий pivot
            Vector2 p = _rectTransform.pivot;

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

            _rectTransform.anchoredPosition = pos;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out _startMousePos
            );
            _startSize = _rectTransform.sizeDelta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnded?.Invoke();
        }
    }
}
