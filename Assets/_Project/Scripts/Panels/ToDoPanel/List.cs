using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TodoBoard
{
    public class List : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _deleteListButton;
        
        private ToDoPanel.TaskListData _taskListData;
        private ToDoPanel.IListDeleter _deleter;
        private IDataUpdater _dataUpdater;
        
        public void Initialize(ToDoPanel.TaskListData taskListData, ToDoPanel.IListDeleter deleter, IDataUpdater dataUpdater)
        {
            _dataUpdater = dataUpdater;
            _deleter = deleter;
            _taskListData = taskListData;
            _inputField.text = taskListData.ListName;
            _deleteListButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _deleteListButton.onClick.AddListener(OnTaskDelete);
            _inputField.onEndEdit.AddListener(OnNameChanged);
        }

        private void OnDisable()
        {
            _deleteListButton.onClick.RemoveListener(OnTaskDelete);
            _inputField.onSubmit.RemoveListener(OnNameChanged);
        }

        private void OnTaskDelete()
        {
            _deleter.DeleteList(_taskListData, gameObject);
        }
        
        private void OnNameChanged(string newName)
        {
            _taskListData.ListName = newName;
            _dataUpdater.UpdateData();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _deleteListButton.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _deleteListButton.gameObject.SetActive(false);
        }
    }
}
