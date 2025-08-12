using System.Collections.Generic;
using Desdinova;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TodoBoard
{
    [RequireComponent(typeof(Button))]
    public class Main : MonoBehaviour, IServiceProvider
    {
        [SerializeField] private SlidingList _slideObject;
        [Space]
        [SerializeField] private ToDoPanel _toDoPanel;
        [SerializeField] private HabitsPanel _habitsPanel;
        [SerializeField] private SettingsPanel _settingsPanel;
        [FormerlySerializedAs("_colourPickerPanel")] [SerializeField] private ColorPickerPanel colorPickerPanel;
        [Space]
        [SerializeField] private TransparentWindowController _windowController;
        
        private Button _mainButton;
        private UserInput _userInput;
        private DataService _dataService;
        private List<Panel> _panels;
        private bool _listExpanded;
        
        public ISaveLoadService SaveLoadService => _dataService;
        public IWindowController WindowController => _windowController;
        public IColorPickerController ColorPickerController => colorPickerPanel;
        public UserInput UserInput => _userInput;

        private void Awake()
        {
            _mainButton = GetComponent<Button>();
            _dataService = new DataService();
            _userInput = new UserInput();
            _userInput.Enable();
            
            _panels = new List<Panel>()
            {
                _toDoPanel,
                _habitsPanel,
                _settingsPanel,
                colorPickerPanel
            };

            foreach (var panel in _panels)
            {
                panel.Initialize(this);
            }
        }
        
        private void OnEnable()
        {
            _mainButton.onClick.AddListener(SwitchListState);
            _userInput.MenuUI.HideAllPanels.performed += HideAllPanelsOnPerformed;
        }
        
        private void OnDisable()
        {
            _mainButton.onClick.RemoveListener(SwitchListState);
            _userInput.MenuUI.HideAllPanels.performed -= HideAllPanelsOnPerformed;
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
        
        private void SwitchListState()
        {
            if (!_slideObject) return;

            if (_listExpanded) _slideObject.Collapse(() => _listExpanded = false);
            else _slideObject.Expand(() => _listExpanded = true);
        }
    }

    public interface IServiceProvider
    {
        ISaveLoadService SaveLoadService { get; }
        IWindowController WindowController { get; }
        IColorPickerController ColorPickerController { get; }
        UserInput UserInput { get; }
    }
}
