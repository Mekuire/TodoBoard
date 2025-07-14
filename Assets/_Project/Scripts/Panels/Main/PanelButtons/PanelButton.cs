using UnityEngine;
using UnityEngine.UI;

namespace TodoBoard
{
    [RequireComponent(typeof(Button))]
    public class PanelButton : MonoBehaviour
    {
        [SerializeField] private Panel _panel;

        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(TogglePanel);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(TogglePanel);
        }

        private void TogglePanel()
        {
            if (_panel.IsActive)
            {
                _panel.Hide();
            }
            else
            {
                _panel.Show();
            }
        }
    }
}
