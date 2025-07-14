using System;
using System.Collections.Generic;
using Desdinova;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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
        [SerializeField] private Slider _fpsFocusedSlider;
        [SerializeField] private TextMeshProUGUI _fpsFocusedNumber;
        [SerializeField] private Slider _fpsUnfocusedSlider;
        [SerializeField] private TextMeshProUGUI _fpsUnfocusedNumber;
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

        private Settings _currentSettings;
        private List<Locale> _locales;
        private UserInput _userInput;
        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
        
        private TransparentWindowController.IWindowController _windowController;
        
        public void Initialize(ISaveLoadService saveLoadService, TransparentWindowController.IWindowController windowController, UserInput input)
        {
            _saveLoadService = saveLoadService;
            _windowController = windowController;
            _userInput = input;
            
            _currentSettings = _saveLoadService.LoadData<Settings>(DATA_KEY) ?? new Settings()
            {
                language = LocalizationSettings.SelectedLocale.LocaleName
            };

            SetupLanguageSettings();
            SetupDisplaySettings();
            SetupInputSettings();
            SetupAudioSettings();
            
            Hide();
        }

        private void OnEnable()
        {
            _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
            _alwaysOnTopToggle.onValueChanged.AddListener(AlwaysOnTopChanged);
            
            _fpsFocusedSlider.onValueChanged.AddListener(OnFPSFocusedChanged);
            _fpsUnfocusedSlider.onValueChanged.AddListener(OnFPSUnFocusedChanged);
            
            _toggleAlwaysOnTopButton.onClick.AddListener(OnToggleAlwaysOnTopPressed);
            _toggleAlwaysOnTopRestoreButton.onClick.AddListener(OnRestoreToggleAlwaysOnTopPressed);
            _hideAllPanelsButton.onClick.AddListener(OnHideAllButtonPressed);
            _hideAllPanelsRestoreButton.onClick.AddListener(OnHideAllButtonRestorePressed);
            
            _clickVolumeSlider.onValueChanged.AddListener(OnClickVolumeChanged);
            _pomodoroAlarmSlider.onValueChanged.AddListener(OnPomodoroVolumeChanged);
        }

        private void OnDisable()
        {
            _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
            _alwaysOnTopToggle.onValueChanged.RemoveListener(AlwaysOnTopChanged);
            
            _fpsFocusedSlider.onValueChanged.RemoveListener(OnFPSFocusedChanged);
            _fpsUnfocusedSlider.onValueChanged.RemoveListener(OnFPSUnFocusedChanged);
            
            _toggleAlwaysOnTopButton.onClick.RemoveListener(OnToggleAlwaysOnTopPressed);
            _toggleAlwaysOnTopRestoreButton.onClick.RemoveListener(OnRestoreToggleAlwaysOnTopPressed);
            _hideAllPanelsButton.onClick.RemoveListener(OnHideAllButtonPressed);
            _hideAllPanelsRestoreButton.onClick.RemoveListener(OnHideAllButtonRestorePressed);
            
            _clickVolumeSlider.onValueChanged.RemoveListener(OnClickVolumeChanged);
            _pomodoroAlarmSlider.onValueChanged.RemoveListener(OnPomodoroVolumeChanged);
            
            Save();
        }

        private void SetupLanguageSettings()
        {
            _locales = LocalizationSettings.AvailableLocales.Locales;

            List<string> options = new();

            foreach (var locale in _locales)
            {
                options.Add(locale.LocaleName);
            }

            int selectedIndex = _locales.FindIndex(l => l.LocaleName == _currentSettings.language);

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
            _alwaysOnTopToggle.isOn = _currentSettings.alwaysOnTop;
            ChangeAlwaysOnTop();
            
            OnFPSFocusedChanged(_currentSettings.fpsFocused);
            OnFPSUnFocusedChanged(_currentSettings.fpsUnFocused);
            
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Application.isFocused ? _currentSettings.fpsFocused : _currentSettings.fpsUnFocused;
        }

        private void SetupInputSettings()
        {
            if (!string.IsNullOrEmpty(_currentSettings.inputOverride))
            {
                _userInput.LoadBindingOverridesFromJson(_currentSettings.inputOverride);
            }
            
            UpdateBindingDisplay(_userInput.UI.HideAllPanels, _hideAllPanelsText);
            UpdateBindingDisplay(_userInput.UI.ToggleAlwaysOnTop, _toggleAlwaysOnTopText);
        }

        private void SetupAudioSettings()
        {
            _clickVolumeSlider.value = _currentSettings.clickVolume;
            _pomodoroAlarmSlider.value = _currentSettings.pomodoroAlarm;
        }

        private void AlwaysOnTopChanged(bool value)
        {
            _currentSettings.alwaysOnTop = value;

            ChangeAlwaysOnTop();

            Save();
        }

        private void ChangeAlwaysOnTop()
        {
            _windowController.SetAlwaysOnTop(_currentSettings.alwaysOnTop);
        }

        private void OnLanguageChanged(int index)
        {
            var locale = _locales[index];
            LocalizationSettings.SelectedLocale = locale;
            _currentSettings.language = locale.LocaleName;

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
            PerformRebind(_userInput.UI.ToggleAlwaysOnTop, _toggleAlwaysOnTopText);
        }
        
        private void OnHideAllButtonPressed()
        {
            PerformRebind(_userInput.UI.HideAllPanels, _hideAllPanelsText);
        }

        private void PerformRebind(InputAction action, TextMeshProUGUI text)
        {
            text.text = "Press key...";
            
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
            _userInput.UI.ToggleAlwaysOnTop.RemoveAllBindingOverrides();
            UpdateBindingDisplay(_userInput.UI.ToggleAlwaysOnTop, _toggleAlwaysOnTopText);
            SaveBindingOverrides();
        }
        
        private void OnHideAllButtonRestorePressed()
        {
            _userInput.UI.HideAllPanels.RemoveAllBindingOverrides();
            UpdateBindingDisplay(_userInput.UI.HideAllPanels, _hideAllPanelsText);
            SaveBindingOverrides();
        }
        
        private void SaveBindingOverrides()
        {
            string inputOverrides = _userInput.SaveBindingOverridesAsJson();
            _currentSettings.inputOverride = inputOverrides;
            Save();
        }

        private void OnFPSFocusedChanged(float value)
        {
            _fpsFocusedNumber.text = value.ToString();
            _currentSettings.fpsFocused = (int)value;
            
            if (Application.isFocused)
            {
                Application.targetFrameRate = _currentSettings.fpsFocused;
            }
        }
        
        private void OnFPSUnFocusedChanged(float value)
        {
            _fpsUnfocusedNumber.text = value.ToString();
            _currentSettings.fpsFocused = (int)value;

            if (!Application.isFocused)
            {
                Application.targetFrameRate = _currentSettings.fpsUnFocused;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Application.targetFrameRate = _currentSettings.fpsFocused;
            }
            else
            {
                Application.targetFrameRate = _currentSettings.fpsUnFocused;
            }
        }

        private void Save()
        {
#if UNITY_EDITOR
            if (_saveDataInEditor == false) return;
#endif
            if (_currentSettings != null)
            {
                _saveLoadService.SaveData(_currentSettings, DATA_KEY);
            }
        }
        
        [Serializable]
        public class Settings
        {
            public string language;
            public bool alwaysOnTop = true;
            public int fpsFocused = 30;
            public int fpsUnFocused = 10;
            public string inputOverride = "";
            public float clickVolume = 10f;
            public float pomodoroAlarm = 10f;
        }
    }
}