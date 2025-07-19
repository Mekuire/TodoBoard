using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TodoBoard
{
    public class Habit : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _deleteHabitButton;
        [SerializeField] private Toggle _toggleMonday;
        [SerializeField] private Toggle _toggleTuesday;
        [SerializeField] private Toggle _toggleWednesday;
        [SerializeField] private Toggle _toggleThursday;
        [SerializeField] private Toggle _toggleFriday;
        [SerializeField] private Toggle _toggleSaturday;
        [SerializeField] private Toggle _toggleSunday;
        [SerializeField] private PointerEvents _pointerEventsObject;
        
        private HabitsPanel.HabitData _habitData;
        private HabitsPanel.IHabitDeleter _deleter;
        private IDataUpdater  _dataUpdater;
        
        public void Initialize(HabitsPanel.HabitData habitData, HabitsPanel.IHabitDeleter deleter, IDataUpdater  dataUpdater)
        {
            _dataUpdater = dataUpdater;
            _deleter = deleter;
            _habitData = habitData;
            _inputField.text = habitData.Name;
            _toggleMonday.isOn = habitData.Monday;
            _toggleTuesday.isOn = habitData.Tuesday;
            _toggleWednesday.isOn = habitData.Wednesday;
            _toggleThursday.isOn = habitData.Thursday;
            _toggleFriday.isOn = habitData.Friday;
            _toggleSaturday.isOn = habitData.Saturday;
            _toggleSunday.isOn = habitData.Sunday;
            
            _deleteHabitButton.gameObject.SetActive(false);
        }

        public void ClearCheckmarks()
        {
            _toggleMonday.isOn = false;
            _toggleTuesday.isOn = false;
            _toggleWednesday.isOn = false;
            _toggleThursday.isOn = false;
            _toggleFriday.isOn = false;
            _toggleSaturday.isOn = false;
            _toggleSunday.isOn = false;
        }
        
        private void OnEnable()
        {
            _deleteHabitButton.onClick.AddListener(OnTaskDelete);
            _inputField.onEndEdit.AddListener(OnNameChanged);
            _pointerEventsObject.OnPointerEntered += OnPointerEnter;
            _pointerEventsObject.OnPointerExited += OnPointerExit;
            _toggleMonday.onValueChanged.AddListener(OnMondayToggleChanged);
            _toggleTuesday.onValueChanged.AddListener(OnTuesdayToggleChanged);
            _toggleWednesday.onValueChanged.AddListener(OnWednesdayToggleChanged);
            _toggleThursday.onValueChanged.AddListener(OnThursdayToggleChanged);
            _toggleFriday.onValueChanged.AddListener(OnFridayToggleChanged);
            _toggleSaturday.onValueChanged.AddListener(OnSaturdayToggleChanged);
            _toggleSunday.onValueChanged.AddListener(OnSundayToggleChanged);
        }

        private void OnTaskDelete()
        {
            _deleter.DeleteHabit(_habitData, gameObject);
        }
        
        private void OnNameChanged(string newName)
        {
            _habitData.Name = newName;
            _dataUpdater.UpdateData();
        }

        private void OnMondayToggleChanged(bool isOn)
        {
            _habitData.Monday = isOn;
            _dataUpdater.UpdateData();
        }
        
        private void OnTuesdayToggleChanged(bool isOn)
        {
            _habitData.Tuesday = isOn;
            _dataUpdater.UpdateData();
        }
        
        private void OnWednesdayToggleChanged(bool isOn)
        {
            _habitData.Wednesday = isOn;
            _dataUpdater.UpdateData();
        }
        
        private void OnThursdayToggleChanged(bool isOn)
        {
            _habitData.Thursday = isOn;
            _dataUpdater.UpdateData();
        }
        
        private void OnFridayToggleChanged(bool isOn)
        {
            _habitData.Friday = isOn;
            _dataUpdater.UpdateData();
        }
        
        private void OnSaturdayToggleChanged(bool isOn)
        {
            _habitData.Saturday = isOn;
            _dataUpdater.UpdateData();
        }
        
        private void OnSundayToggleChanged(bool isOn)
        {
            _habitData.Sunday = isOn;
            _dataUpdater.UpdateData();
        }
        
        private void OnDisable()
        {
            _deleteHabitButton.onClick.RemoveListener(OnTaskDelete);
            _inputField.onSubmit.RemoveListener(OnNameChanged);
            _pointerEventsObject.OnPointerEntered -= OnPointerEnter;
            _pointerEventsObject.OnPointerExited -= OnPointerExit;
            _toggleMonday.onValueChanged.RemoveListener(OnMondayToggleChanged);
            _toggleTuesday.onValueChanged.RemoveListener(OnTuesdayToggleChanged);
            _toggleWednesday.onValueChanged.RemoveListener(OnWednesdayToggleChanged);
            _toggleThursday.onValueChanged.RemoveListener(OnThursdayToggleChanged);
            _toggleFriday.onValueChanged.RemoveListener(OnFridayToggleChanged);
            _toggleSaturday.onValueChanged.RemoveListener(OnSaturdayToggleChanged);
            _toggleSunday.onValueChanged.RemoveListener(OnSundayToggleChanged);
        }

        public void OnPointerEnter()
        {
            _deleteHabitButton.gameObject.SetActive(true);
        }

        public void OnPointerExit()
        {
            _deleteHabitButton.gameObject.SetActive(false);
        }
    }
}
