using System;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace TodoBoard
{
    public class ToDoPanel : Panel, ToDoPanel.ITaskDeleter, ToDoPanel.IListDeleter, ToDoPanel.IListOpener, IDataUpdater
    {
        private const string DATA_KEY = "todo-list";

        [Header("Main Tasks Menu")] 
        [SerializeField] private Button _editListButton;
        [SerializeField] private Button _addTaskButton;
        [SerializeField] private ListButton _listButtonPrefab;
        [SerializeField] private RectTransform _listButtonsParent;
        [SerializeField] private ScrollRect _scrollView;
        [SerializeField] private GameObject _scrollContextPrefab;
        [SerializeField] private TaskItem taskItemPrefab;
        [Space] 
        [Header("Lists Edit Menu")] 
        [SerializeField] private ListItem listItemPrefab;
        [SerializeField] private Button _confirmListsButton;
        [SerializeField] private Button _addListButton;
        [SerializeField] private ScrollRect _listScrollView;
        [Space] 
        [Header("Data")] 
        [SerializeField] private bool _saveDataInEditor = true;

        private const int MaxLists = 5;
        private ToDoData _currentData;
        private string _currentListId;

        public void Initialize(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            
            Setup();
            
            Hide();
        }

        private void Setup()
        {
            _currentData = _saveLoadService.LoadData<ToDoData>(DATA_KEY);

            _currentData ??= GetBasicToDoList();
            _currentListId = _currentData.TaskLists[0].Id;

            ToggleListEditMenu(false);
            ToggleTasksMenu(true);

            SetupToDoList(_currentData, _currentListId);

            PreventAddingExtraList();
        }

        private void OnEnable()
        {
            _addTaskButton.onClick.AddListener(OnAddTask);
            _addListButton.onClick.AddListener(OnAddList);
            _editListButton.onClick.AddListener(OnEditLists);
            _confirmListsButton.onClick.AddListener(OnConfirmLists);
        }

        private void OnDisable()
        {
            _addTaskButton.onClick.RemoveListener(OnAddTask);
            _addListButton.onClick.RemoveListener(OnAddList);
            _editListButton.onClick.RemoveListener(OnEditLists);
            _confirmListsButton.onClick.RemoveListener(OnConfirmLists);
        }

        private void OnAddTask()
        {
            TaskListData taskList = _currentData.TaskLists.Find(l => l.Id == _currentListId);
            TaskData taskData = new TaskData();
            CreateTaskObject(taskData);
            taskList.Tasks.Add(taskData);

            Save();
        }

        private void OnAddList()
        {
            TaskListData taskList = new TaskListData
            {
                Tasks = new List<TaskData> { new TaskData() }
            };
            _currentData.TaskLists.Add(taskList);
            CreateListObject(taskList);

            PreventAddingExtraList();

            Save();
        }

        private void PreventAddingExtraList()
        {
            if (_currentData.TaskLists.Count >= MaxLists)
            {
                _addListButton.interactable = false;
            }
            else
            {
                _addListButton.interactable = true;
            }
        }

        private void OnEditLists()
        {
            ToggleTasksMenu(false);
            ToggleListEditMenu(true);
            SetupListsEditMenu(_currentData);

            Save();
        }

        private void OnConfirmLists()
        {
            TaskListData taskList = _currentData.TaskLists.Find(l => l.Id == _currentListId);
            if (taskList == null)
            {
                taskList = _currentData.TaskLists[0];
                _currentListId = taskList.Id;
            }

            ToggleTasksMenu(true);
            ToggleListEditMenu(false);
            SetupToDoList(_currentData, _currentListId);

            Save();
        }

        private void ToggleTasksMenu(bool value)
        {
            _listButtonsParent.gameObject.SetActive(value);
            _scrollView.gameObject.SetActive(value);
            _addTaskButton.gameObject.SetActive(value);
            _editListButton.gameObject.SetActive(value);
        }

        private void ToggleListEditMenu(bool value)
        {
            _addListButton.gameObject.SetActive(value);
            _listScrollView.gameObject.SetActive(value);
            _confirmListsButton.gameObject.SetActive(value);
        }

        private ToDoData GetBasicToDoList()
        {
            ToDoData data = new ToDoData
            {
                TaskLists = new List<TaskListData>()
            };

            TaskListData listData = new TaskListData
            {
                Tasks = new List<TaskData> { new() }
            };

            data.TaskLists = new List<TaskListData> { listData };

            return data;
        }

        private void SetupToDoList(ToDoData data, string id)
        {
            ClearTasksPanel();

            foreach (TaskListData list in data.TaskLists)
            {
                CreateListButton(list);

                if (list.Id != id) continue;

                foreach (TaskData taskData in list.Tasks)
                {
                    TaskItem taskItem = Instantiate(taskItemPrefab, _scrollView.content);
                    taskItem.Initialize(taskData, deleter: this, dataUpdater: this);
                }
            }
        }

        private void SetupTasks(TaskListData taskList)
        {
            DestroyChildObjects(_scrollView.content);

            foreach (TaskData taskData in taskList.Tasks)
            {
                TaskItem taskItem = Instantiate(taskItemPrefab, _scrollView.content);
                taskItem.Initialize(taskData, deleter: this, dataUpdater: this);
            }
        }

        private void SetupListsEditMenu(ToDoData data)
        {
            ClearListsPanel();

            foreach (TaskListData listData in data.TaskLists)
            {
                ListItem listItem = Instantiate(listItemPrefab, _listScrollView.content);
                listItem.Initialize(listData, deleter: this, dataUpdater: this);
            }
        }

        private void CreateTaskObject(TaskData data)
        {
            TaskItem taskItem = Instantiate(taskItemPrefab, _scrollView.content);

            taskItem.Initialize(data, deleter: this, dataUpdater: this);
        }

        private void CreateListObject(TaskListData taskList)
        {
            ListItem listItem = Instantiate(listItemPrefab, _listScrollView.content);

            listItem.Initialize(taskList, deleter: this, dataUpdater: this);
        }

        private void CreateListButton(TaskListData list)
        {
            ListButton listButton = Instantiate(_listButtonPrefab, _listButtonsParent);
            listButton.Initialize(list, this);
        }

        private void ClearTasksPanel()
        {
            DestroyChildObjects(_listButtonsParent);
            DestroyChildObjects(_scrollView.content);
        }

        private void ClearListsPanel()
        {
            DestroyChildObjects(_listScrollView.content);
        }
        
        private void Save()
        {
#if UNITY_EDITOR
            if (_saveDataInEditor == false) return;
#endif
            if (_currentData != null)
            {
                _saveLoadService.SaveData(_currentData, DATA_KEY);
            }
        }

        [Serializable]
        public class ToDoData
        {
            public List<TaskListData> TaskLists;
        }

        [Serializable]
        public class TaskListData
        {
            public string ListName = "My List";
            public string Id = Guid.NewGuid().ToString();
            public List<TaskData> Tasks;
        }

        [Serializable]
        public class TaskData
        {
            public string Name = "My new task...";
            public bool IsCompleted = false;
        }

        public interface ITaskDeleter
        {
            public void DeleteTask(TaskData taskData, GameObject task);
        }

        public interface IListDeleter
        {
            public void DeleteList(TaskListData taskList, GameObject list);
        }

        public interface IListOpener
        {
            public void OpenTaskList(TaskListData taskList);
        }
        
        public void DeleteTask(TaskData taskData, GameObject task)
        {
            TaskListData taskList = _currentData.TaskLists.Find(l => l.Id == _currentListId);

            if (taskList == null)
            {
                Debug.LogWarning("Task list not found");
                return;
            }

            if (!taskList.Tasks.Contains(taskData))
            {
                Debug.LogWarning("There is no such task to delete");
                return;
            }

            taskList.Tasks.Remove(taskData);
            Destroy(task);

            Save();
        }

        public void DeleteList(TaskListData taskList, GameObject list)
        {
            if (taskList.Id == _currentListId)
            {
                _currentListId = _currentData.TaskLists[0].Id;
            }

            if (!_currentData.TaskLists.Contains(taskList))
            {
                Debug.LogWarning("There is no such list to delete");
                return;
            }

            _currentData.TaskLists.Remove(taskList);
            Destroy(list);

            PreventAddingExtraList();

            Save();
        }

        public void OpenTaskList(TaskListData taskList)
        {
            _currentListId = taskList.Id;
            SetupTasks(taskList);
        }

        public void UpdateData()
        {
            Save();
        }
    }
}