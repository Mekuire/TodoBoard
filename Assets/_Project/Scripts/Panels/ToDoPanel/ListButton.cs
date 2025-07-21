using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TodoBoard
{
    [RequireComponent(typeof(Button))]
    public class ListButton : MonoBehaviour
    {
        private Button _button;
        private TaskListData  _taskList;
        private TextMeshProUGUI _label;
        private IListOpener  _listOpener;
        
        public void Initialize(TaskListData taskListData, IListOpener listOpener)
        {
            _listOpener = listOpener;
            _taskList = taskListData;
            _button = GetComponent<Button>();
            _label = GetComponentInChildren<TextMeshProUGUI>();
            _label.text = taskListData.ListName;
            _button.onClick.AddListener(OnListOpen);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(OnListOpen);
        }

        private void OnListOpen()
        {
            _listOpener.OpenTaskList(_taskList);
        }
    }
}
