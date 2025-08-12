using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    [RequireComponent(typeof(RectTransform))]
    public class SVPickControl : MonoBehaviour, IDragHandler, IPointerClickHandler
    {
        [SerializeField] private RectTransform _picker;
        
        private RectTransform _boundsRect;
        private float _minV;
        
        public Action<float, float> OnValueChanged;
        
        public void Initialize(float minV)
        {
            _minV = minV;
            _boundsRect = GetComponent<RectTransform>();
        }

        public void SetPickerPositionByColor(HSVColor color)
        {
            float halfWidth = _boundsRect.sizeDelta.x * 0.5f;
            float halfHeight = _boundsRect.sizeDelta.y * 0.5f;
            float x = Mathf.Lerp(-halfWidth, halfWidth, color.s);
            float y = Mathf.Lerp(-halfHeight, halfHeight, Mathf.InverseLerp(_minV, 1f, color.v));
            _picker.localPosition = new Vector3(x, y, 0);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            UpdateColor(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UpdateColor(eventData);
        }

        private void UpdateColor(PointerEventData eventData)
        {
            // from canvas space to bound rect space
            Vector3 pos = _boundsRect.InverseTransformPoint(eventData.position);
            
            // half size of bound rect
            float halfSizeX = _boundsRect.sizeDelta.x * 0.5f;
            float halfSizeY = _boundsRect.sizeDelta.y * 0.5f;
            
            // clamping in bounds between sides (anchor needs to be center)
            pos.x = Mathf.Clamp(pos.x, -halfSizeX, halfSizeX);
            pos.y = Mathf.Clamp(pos.y, -halfSizeY, halfSizeY);
            
            _picker.localPosition = pos;
            
            // prevent from -position: (-127, 127) converts to (0, 254)
            float x = pos.x + halfSizeX;
            float y = pos.y + halfSizeY;
            
            // 150 / 254 = normalized value
            float xNormalized = x / _boundsRect.sizeDelta.x;
            float yNormalized = y / _boundsRect.sizeDelta.y;
            
            OnValueChanged?.Invoke(xNormalized, yNormalized);
        }
    }
}
