using Desdinova;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TodoBoard
{
    [RequireComponent(typeof(Button))]
    public class Main : MonoBehaviour, ISaveLoadService
    {
        [SerializeField] private SlidingList _slideObject;
        [SerializeField] private ToDoPanel _toDoPanel;
        [SerializeField] private HabitsPanel _habitsPanel;
        [SerializeField] private SettingsPanel _settingsPanel;
        [SerializeField] private TransparentWindowController _windowController;
        
        private Button _button;
        private bool _listExpanded;
        private UserInput  _userInput;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _userInput = new UserInput();
            _userInput.Enable();
            
            _toDoPanel.Initialize(this);
            _habitsPanel.Initialize(this);
            _settingsPanel.Initialize(this, _windowController, _userInput);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(SwitchListState);
            _userInput.UI.HideAllPanels.performed += HideAllPanelsOnperformed;
        }

        private void HideAllPanelsOnperformed(InputAction.CallbackContext obj)
        {
            if (_habitsPanel.IsActive) _habitsPanel.Hide();
            if (_settingsPanel.IsActive)_settingsPanel.Hide();
            if (_toDoPanel.IsActive) _toDoPanel.Hide();
            
            if (!_slideObject) return;
            _slideObject.Collapse(() => _listExpanded = false);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(SwitchListState);
            _userInput.UI.HideAllPanels.performed -= HideAllPanelsOnperformed;
        }

        public void SaveData<T>(T data, string fileName)
        {
            DataService.SaveData(data, fileName);
        }

        public T LoadData<T>(string fileName)
        {
            DataService.LoadData<T>(fileName, out var data);
            return data;
        }
        
        private void SwitchListState()
        {
            if (!_slideObject) return;

            if (_listExpanded)
            {
                _slideObject.Collapse(() => _listExpanded = false);
            }
            else
            {
                _slideObject.Expand(() => _listExpanded = true);
            }
        }
    }
}
