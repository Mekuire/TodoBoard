using System;
using System.Collections.Generic;
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
        [Space]
        [SerializeField] private ToDoPanel _toDoPanel;
        [SerializeField] private HabitsPanel _habitsPanel;
        [SerializeField] private SettingsPanel _settingsPanel;
        [Space]
        [SerializeField] private TransparentWindowController _windowController;
        
        private Button _button;
        private UserInput  _userInput;
        private bool _listExpanded;
        private List<Panel> _panels;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _userInput = new UserInput();
            _userInput.Enable();
            
            _toDoPanel.Initialize(this);
            _habitsPanel.Initialize(this);
            _settingsPanel.Initialize(this, _windowController, _userInput);

            _panels = new List<Panel>()
            {
                _toDoPanel,
                _habitsPanel,
                _settingsPanel
            };
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(SwitchListState);
            _userInput.MenuUI.HideAllPanels.performed += HideAllPanelsOnPerformed;
        }

        private void HideAllPanelsOnPerformed(InputAction.CallbackContext obj)
        {
            foreach (Panel panel in _panels)
            {
                if (panel.IsActive) panel.Hide();
            }
            
            if (!_slideObject) return;
            _slideObject.Collapse(() => _listExpanded = false);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(SwitchListState);
            _userInput.MenuUI.HideAllPanels.performed -= HideAllPanelsOnPerformed;
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

            if (_listExpanded) _slideObject.Collapse(() => _listExpanded = false);
            else _slideObject.Expand(() => _listExpanded = true);
        }
    }
}
