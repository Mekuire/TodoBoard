using System.Collections.Generic;
using Desdinova;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TodoBoard
{
    public class SettingsPanel : Panel
    {
        private const string DATA_KEY = "settings";

        [Header("Language")] 
        [SerializeField] private TMP_Dropdown _languageDropdown;
        [Header("Display")] 
        [SerializeField] private Toggle _alwaysOnTopToggle;
        [FormerlySerializedAs("_fpsFocusedSlider")] 
        [SerializeField] private Slider _targetFpsSlider;
        [SerializeField] private TextMeshProUGUI _fpsFocusedNumber;
        [Header("Colors")]
        [SerializeField] private Material _uiMaterial;
        [SerializeField] private Button _changeBgColorButton;
        [Header("Shortcuts")] 
        [SerializeField] private Button _toggleAlwaysOnTopButton;
        [SerializeField] private TextMeshProUGUI _toggleAlwaysOnTopText;
        [SerializeField] private Button _toggleAlwaysOnTopRestoreButton;
        [SerializeField] private Button _hideAllPanelsButton;
        [SerializeField] private TextMeshProUGUI _hideAllPanelsText;
        [SerializeField] private Button _hideAllPanelsRestoreButton;
        [Header("Audio")] 
        [SerializeField] private Slider _clickVolumeSlider;
        [SerializeField] private TextMeshProUGUI _clickVolumeNumber;
        [SerializeField] private Slider _pomodoroAlarmSlider;
        [SerializeField] private TextMeshProUGUI _pomodoroAlarmNumber;
        [Header("Data")] 
        [SerializeField] private bool _saveDataInEditor = true;

        private SettingsData _currentSettingsData;
        private List<Locale> _locales;
        private UserInput _userInput;
        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
        private ColourPickerController _colourPickerController;
        private IWindowController _windowController;
        private GameObject _rootGO;
        
        public void Initialize(ISaveLoadService saveLoadService, IWindowController windowController, ColourPickerController colourController, UserInput input)
        {
            _saveLoadService = saveLoadService;
            _windowController = windowController;
            _colourPickerController = colourController;
            _userInput = input;
            _rootGO = transform.root.gameObject;
            
            _currentSettingsData = _saveLoadService.LoadData<SettingsData>(DATA_KEY) ?? new SettingsData()
            {
                language = LocalizationSettings.SelectedLocale.LocaleName
            };

            SetupLanguageSettings();
            SetupDisplaySettings();
            SetupColorSettings();
            SetupInputSettings();
            SetupAudioSettings();
            
            Hide();
        }

        private void OnEnable()
        {
            _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
            
            _targetFpsSlider.onValueChanged.AddListener(OnTargetFPSChanged);
            
            _toggleAlwaysOnTopButton.onClick.AddListener(OnToggleAlwaysOnTopPressed);
            _toggleAlwaysOnTopRestoreButton.onClick.AddListener(OnRestoreToggleAlwaysOnTopPressed);
            _hideAllPanelsButton.onClick.AddListener(OnHideAllButtonPressed);
            _hideAllPanelsRestoreButton.onClick.AddListener(OnHideAllButtonRestorePressed);
            
            _clickVolumeSlider.onValueChanged.AddListener(OnClickVolumeChanged);
            _pomodoroAlarmSlider.onValueChanged.AddListener(OnPomodoroVolumeChanged);
            
            _changeBgColorButton.onClick.AddListener(OnChangeBgColorPressed);
        }
        
        private void OnDisable()
        {
            _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
            
            _targetFpsSlider.onValueChanged.RemoveListener(OnTargetFPSChanged);
            
            _toggleAlwaysOnTopButton.onClick.RemoveListener(OnToggleAlwaysOnTopPressed);
            _toggleAlwaysOnTopRestoreButton.onClick.RemoveListener(OnRestoreToggleAlwaysOnTopPressed);
            _hideAllPanelsButton.onClick.RemoveListener(OnHideAllButtonPressed);
            _hideAllPanelsRestoreButton.onClick.RemoveListener(OnHideAllButtonRestorePressed);
            
            _clickVolumeSlider.onValueChanged.RemoveListener(OnClickVolumeChanged);
            _pomodoroAlarmSlider.onValueChanged.RemoveListener(OnPomodoroVolumeChanged);
            
            _changeBgColorButton.onClick.RemoveListener(OnChangeBgColorPressed);

            //Save();
        }

        private void OnDestroy()
        {
            _userInput.MenuUI.ToggleAlwaysOnTop.performed -= ToggleAlwaysOnTopOnPerformed;
            _alwaysOnTopToggle.onValueChanged.RemoveListener(AlwaysOnTopChanged);
            Application.focusChanged -= OnApplicationFocusChanged;
        }

        private void SetupLanguageSettings()
        {
            _locales = LocalizationSettings.AvailableLocales.Locales;

            List<string> options = new();

            foreach (var locale in _locales)
            {
                options.Add(locale.LocaleName);
            }

            int selectedIndex = _locales.FindIndex(l => l.LocaleName == _currentSettingsData.language);

            if (selectedIndex == -1)
                selectedIndex = _locales.IndexOf(LocalizationSettings.SelectedLocale);

            LocalizationSettings.SelectedLocale = _locales[selectedIndex];
            
            // refresh
            _languageDropdown.ClearOptions();
            _languageDropdown.AddOptions(options);
            _languageDropdown.value = selectedIndex;
            _languageDropdown.RefreshShownValue();
        }

        private void SetupDisplaySettings()
        {
            Application.focusChanged += OnApplicationFocusChanged;
            _userInput.MenuUI.ToggleAlwaysOnTop.performed += ToggleAlwaysOnTopOnPerformed;
            _alwaysOnTopToggle.onValueChanged.AddListener(AlwaysOnTopChanged);

            _alwaysOnTopToggle.SetIsOnWithoutNotify(_currentSettingsData.alwaysOnTop);
            ChangeAlwaysOnTop();
            
            _targetFpsSlider.SetValueWithoutNotify(_currentSettingsData.fpsFocused);
            _fpsFocusedNumber.text = (_currentSettingsData.fpsFocused * 10).ToString();
            
            SetFPS(_currentSettingsData.fpsFocused);
            
            QualitySettings.vSyncCount = 0;
            OnApplicationFocusChanged(Application.isFocused);
        }

        private void ToggleAlwaysOnTopOnPerformed(InputAction.CallbackContext obj)
        {
            _currentSettingsData.alwaysOnTop = !_currentSettingsData.alwaysOnTop;
            _alwaysOnTopToggle.isOn = _currentSettingsData.alwaysOnTop;
        }

        private void SetupInputSettings()
        {
            if (!string.IsNullOrEmpty(_currentSettingsData.inputOverride))
            {
                _userInput.LoadBindingOverridesFromJson(_currentSettingsData.inputOverride);
            }
            
            UpdateBindingDisplay(_userInput.MenuUI.HideAllPanels, _hideAllPanelsText);
            UpdateBindingDisplay(_userInput.MenuUI.ToggleAlwaysOnTop, _toggleAlwaysOnTopText);
        }

        private void SetupAudioSettings()
        {
            // _clickVolumeSlider.value = _currentSettingsData.clickVolume;
            // _pomodoroAlarmSlider.value = _currentSettingsData.pomodoroAlarm;
        }
        
        private void SetupColorSettings()
        {
            _colourPickerController.gameObject.SetActive(false);
            HSVColor savedColor = _currentSettingsData.bgColor;
            _uiMaterial.color = Color.HSVToRGB(savedColor.h, savedColor.s, savedColor.v);
        }
        
        private void AlwaysOnTopChanged(bool value)
        {
            _currentSettingsData.alwaysOnTop = value;

            ChangeAlwaysOnTop();

            Save();
        }

        private void ChangeAlwaysOnTop()
        {
            _windowController.SetAlwaysOnTop(_currentSettingsData.alwaysOnTop);
        }

        private void OnLanguageChanged(int index)
        {
            var locale = _locales[index];
            LocalizationSettings.SelectedLocale = locale;
            _currentSettingsData.language = locale.LocaleName;

            Save();
        }
        
        private void OnChangeBgColorPressed()
        {
            _changeBgColorButton.interactable = false;
            
            _colourPickerController.SetColour(_currentSettingsData.bgColor);
            _colourPickerController.Show();
            
            _colourPickerController.OnColorChanged += ColourPickerController_OnColorChanged;
            _colourPickerController.OnColorSelectionDone += ColourPicker_OnBgColorSelectionDone;
        }

        private void ColourPickerController_OnColorChanged(float h, float s, float v)
        {
            _uiMaterial.color = Color.HSVToRGB(h, s, v);
            _rootGO.SetActive(false);
            _rootGO.SetActive(true);
        }

        private void ColourPicker_OnBgColorSelectionDone(HSVColor color)
        {
            _changeBgColorButton.interactable = true;
            _currentSettingsData.bgColor = color;
            _colourPickerController.Hide();
            
            ColourPickerController_OnColorChanged(color.h, color.s, color.v);
            
            _colourPickerController.OnColorSelectionDone -= ColourPicker_OnBgColorSelectionDone;
            _colourPickerController.OnColorChanged -= ColourPickerController_OnColorChanged;
            
            Save();
        }
        
        private void OnClickVolumeChanged(float value)
        {
            
        }
        
        private void OnPomodoroVolumeChanged(float value)
        {
            
        }

        private void OnToggleAlwaysOnTopPressed()
        {
            PerformRebind(_userInput.MenuUI.ToggleAlwaysOnTop, _toggleAlwaysOnTopText);
        }
        
        private void OnHideAllButtonPressed()
        {
            PerformRebind(_userInput.MenuUI.HideAllPanels, _hideAllPanelsText);
        }

        private void PerformRebind(InputAction action, TextMeshProUGUI text)
        {
            text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Settings", "settings.pressKey");
            
            _rebindingOperation?.Dispose();

            action.Disable();
            _rebindingOperation = action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete((op) =>
                {
                    op.Dispose();
                    SaveBindingOverrides();
                    UpdateBindingDisplay(action, text);
                    action.Enable();
                })
                .Start();
        }
        
        private void UpdateBindingDisplay(InputAction action, TextMeshProUGUI text)
        {
            var binding = action.bindings[0];

            if (!string.IsNullOrEmpty(binding.overridePath))
            {
                text.text = InputControlPath.ToHumanReadableString(
                    binding.overridePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice
                );
            }
            else
            {
                text.text = InputControlPath.ToHumanReadableString(
                    binding.effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice
                );
            }
        }
        
        private void OnRestoreToggleAlwaysOnTopPressed()
        {
            _userInput.MenuUI.ToggleAlwaysOnTop.RemoveAllBindingOverrides();
            UpdateBindingDisplay(_userInput.MenuUI.ToggleAlwaysOnTop, _toggleAlwaysOnTopText);
            SaveBindingOverrides();
        }
        
        private void OnHideAllButtonRestorePressed()
        {
            _userInput.MenuUI.HideAllPanels.RemoveAllBindingOverrides();
            UpdateBindingDisplay(_userInput.MenuUI.HideAllPanels, _hideAllPanelsText);
            SaveBindingOverrides();
        }
        
        private void SaveBindingOverrides()
        {
            string inputOverrides = _userInput.SaveBindingOverridesAsJson();
            _currentSettingsData.inputOverride = inputOverrides;
            Save();
        }

        private void OnTargetFPSChanged(float value)
        {
            SetFPS(value);
            Save();
        }

        private void SetFPS(float value)
        {
            int intValue = (int)value;
            int fps = intValue * 10;
            _fpsFocusedNumber.text = fps.ToString();
            _currentSettingsData.fpsFocused = intValue;
            
            Application.targetFrameRate = fps;
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            Application.targetFrameRate = (hasFocus ? _currentSettingsData.fpsFocused * 10 : _currentSettingsData.fpsUnFocused);
        }

        private void Save()
        {
#if UNITY_EDITOR
            if (_saveDataInEditor == false) return;
#endif
            if (_currentSettingsData != null)
            {
                _saveLoadService.SaveData(_currentSettingsData, DATA_KEY);
            }
        }
    }
}