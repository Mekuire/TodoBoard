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

        public Action<float, float> OnValueChanged;
        
        private void Awake()
        {
            _boundsRect = GetComponent<RectTransform>();
        }

        public void SetPickerPositionByColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            float x = s * _boundsRect.sizeDelta.x - _boundsRect.sizeDelta.x / 2;
            float y = v * _boundsRect.sizeDelta.y - _boundsRect.sizeDelta.y / 2;
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
            float deltaX = _boundsRect.sizeDelta.x * 0.5f;
            float deltaY = _boundsRect.sizeDelta.y * 0.5f;
            
            // clamping in bounds between sides (anchor needs to be center)
            pos.x = Mathf.Clamp(pos.x, -deltaX, deltaX);
            pos.y = Mathf.Clamp(pos.y, -deltaY, deltaY);
            
            _picker.localPosition = pos;
            
            // prevent from -position: (-127, 127) converts to (0, 254)
            float x = pos.x + deltaX;
            float y = pos.y + deltaY;
            
            // 150 / 254 = normalized value
            float xNormalized = x / _boundsRect.sizeDelta.x;
            float yNormalized = y / _boundsRect.sizeDelta.y;
            
            OnValueChanged?.Invoke(xNormalized, yNormalized);
        }
    }
}
