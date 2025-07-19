using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TodoBoard
{
    public class TaskItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Button _deleteTaskButton;
        
        private ToDoPanel.TaskData _taskData;
        private ToDoPanel.ITaskDeleter _deleter;
        private IDataUpdater  _dataUpdater;
        
        public void Initialize(ToDoPanel.TaskData taskData, ToDoPanel.ITaskDeleter deleter, IDataUpdater dataUpdater)
        {
            _dataUpdater = dataUpdater;
            _deleter = deleter;
            _taskData = taskData;
            _inputField.text = taskData.Name;
            _toggle.isOn = taskData.IsCompleted;
            _deleteTaskButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _deleteTaskButton.onClick.AddListener(OnTaskDelete);
            _inputField.onEndEdit.AddListener(OnNameChanged);
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnTaskDelete()
        {
            _deleter.DeleteTask(_taskData, gameObject);
        }
        
        private void OnNameChanged(string newName)
        {
            _taskData.Name = newName;
            _dataUpdater.UpdateData();
        }

        private void OnToggleValueChanged(bool isOn)
        {
            _taskData.IsCompleted = isOn;
            _dataUpdater.UpdateData();
        }
        
        private void OnDisable()
        {
            _deleteTaskButton.onClick.RemoveListener(OnTaskDelete);
            _inputField.onSubmit.RemoveListener(OnNameChanged);
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _deleteTaskButton.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _deleteTaskButton.gameObject.SetActive(false);
        }
    }
}
