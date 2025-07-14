using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TodoBoard
{
    public class SlidingList : MonoBehaviour
    {
        [SerializeField] private RectTransform _slideObject;
        [SerializeField] private Vector2 _targetPosition;
        [SerializeField] private Vector2 _retractedPosition;
        [SerializeField] private Ease _curveEase = Ease.Linear;
        [SerializeField] private float _duration = 0.5f;

        private Tween _tween;
        private Selectable[] _selectables;

        private void Start()
        {
            _slideObject.anchoredPosition = _retractedPosition;
            _selectables = GetComponentsInChildren<Selectable>();
            ToggleInteractables(false);
        }

        public void Expand(Action onComplete)
        {
            if (_tween != null && _tween.IsActive() && _tween.IsPlaying()) return;

            _tween?.Kill();
            _tween = _slideObject.DOAnchorPos(_targetPosition, _duration)
                .SetEase(_curveEase)
                .OnComplete(() =>
                {
                    ToggleInteractables(true);
                    onComplete?.Invoke();
                })
                .SetLink(_slideObject.gameObject);
        }

        public void Collapse(Action onComplete)
        {
            if (_tween != null && _tween.IsActive() && _tween.IsPlaying()) return;

            _tween?.Kill();
            _tween = _slideObject.DOAnchorPos(_retractedPosition, _duration)
                .SetEase(_curveEase)
                .OnStart(() => ToggleInteractables(false))
                .OnComplete(() => onComplete?.Invoke())
                .SetLink(_slideObject.gameObject);
        }

        private void ToggleInteractables(bool value)
        {
            foreach (Selectable selectable in _selectables)
            {
                selectable.interactable = value;
            }
        }
    }
}