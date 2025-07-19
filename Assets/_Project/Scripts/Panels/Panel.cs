using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TodoBoard
{
    public abstract class Panel : MonoBehaviour
    {
        [SerializeField] protected bool _hideOnOutsideClick = false;
        
        protected ISaveLoadService _saveLoadService;
        private bool _isActive;
        public bool IsActive => _isActive;
        
        public virtual void Show(Action onComplete = null)
        {
            _isActive = true;
            gameObject.SetActive(true);
            onComplete?.Invoke();
        }

        public virtual void Hide(Action onComplete = null)
        {
            _isActive = false;
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
        
        protected void DestroyChildObjects(RectTransform rectTransform)
        {
            for (int i = rectTransform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(rectTransform.GetChild(i).gameObject);
            }
        }

        // public void HideIfClickedOutside()
        // {
        //     if (_hideOnOutsideClick && !EventSystem.current.IsPointerOverGameObject())
        //     {
        //         Debug.Log("hide");
        //         Hide();
        //     }
        // }
    }
}
