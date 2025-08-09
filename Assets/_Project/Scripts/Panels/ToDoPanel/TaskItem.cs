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
        [SerializeField] private PointerEvents _pointerEvents;
        [SerializeField] private Image _bgImage;
        [SerializeField, Range(0f, 1f)] private float _dragBgAlpha = 0.5f;
        
        private TaskData _taskData;
        private ITaskDeleter _deleter;
        private IDataUpdater  _dataUpdater;
        private ITaskReplacer _replacer;
        
        public TaskData TaskData => _taskData;
        
        public void Initialize(TaskData taskData, ITaskDeleter deleter, IDataUpdater dataUpdater, ITaskReplacer taskReplacer)
        {
            _replacer = taskReplacer;
            _dataUpdater = dataUpdater;
            _deleter = deleter;
            _taskData = taskData;
            _inputField.text = taskData.Name;
            _toggle.isOn = taskData.IsCompleted;
            _deleteTaskButton.gameObject.SetActive(false);
            _bgImage.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
        }

        private void OnEnable()
        {
            _pointerEvents.OnDragBegin += OnDragBegin;
            _pointerEvents.OnDraging += OnDraging;
            _pointerEvents.OnDragEnd += OnDragEnded;
            
            _deleteTaskButton.onClick.AddListener(OnTaskDelete);
            _inputField.onEndEdit.AddListener(OnNameChanged);
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDragEnded(PointerEventData obj)
        {
           _replacer.OnDragComplete();
           _bgImage.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
        }

        private void OnDraging(PointerEventData obj)
        {
            _replacer.OnDragTask(this, obj.position);
        }

        private void OnDragBegin(PointerEventData obj)
        {
            OnPointerExit(null);
            _replacer.OnDragStart();
            _bgImage.color = new Color(Color.white.r, Color.white.g, Color.white.b, _dragBgAlpha);
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
            _pointerEvents.OnDragBegin -= OnDragBegin;
            _pointerEvents.OnDraging -= OnDraging;
            _pointerEvents.OnDragEnd -= OnDragEnded;

            _deleteTaskButton.onClick.RemoveListener(OnTaskDelete);
            _inputField.onSubmit.RemoveListener(OnNameChanged);
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_replacer.IsDragging) return;
            
            _deleteTaskButton.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _deleteTaskButton.gameObject.SetActive(false);
        }
    }
}
