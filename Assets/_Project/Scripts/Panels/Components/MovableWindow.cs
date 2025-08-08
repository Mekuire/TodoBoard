using System;
using UnityEngine;

namespace TodoBoard
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class MovableWindow : MonoBehaviour
    {
        [SerializeField] protected RectTransform _uiElement;

        protected RectTransform _canvasRect;
        
        protected virtual void Awake()
        {
            if (_uiElement == null)
            {
                _uiElement = GetComponent<RectTransform>();
            }
            
            _canvasRect = _uiElement.root as RectTransform;
            
            Vector2 savedPosition = _uiElement.position;
            // setting anchor to center for convenient move
            _uiElement.anchorMin = new Vector2(0.5f, 0.5f);
            _uiElement.anchorMax = new Vector2(0.5f, 0.5f);
            _uiElement.position = savedPosition;
        }

        protected void PreventFromMoveOutside()
        {
            Vector2 size = _uiElement.sizeDelta;
            Vector2 pos = _uiElement.anchoredPosition;
            Vector2 canvasSize = _canvasRect.sizeDelta;

            float canvasHalfW = canvasSize.x * 0.5f;
            float canvasHalfH = canvasSize.y * 0.5f;

            Vector2 p = _uiElement.pivot;

            float minX = -canvasHalfW + size.x * p.x;
            float maxX =  canvasHalfW - size.x * (1 - p.x);
            float minY = -canvasHalfH + size.y * p.y;
            float maxY =  canvasHalfH - size.y * (1 - p.y);

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            _uiElement.anchoredPosition = pos;
        }
    }
}
