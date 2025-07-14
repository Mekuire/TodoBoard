using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TodoBoard
{
    public class HabitsPanel : Panel, IDataUpdater, HabitsPanel.IHabitDeleter
    {
        private const string DATA_KEY = "habits";

        [SerializeField] private Habit _habitPrefab;
        [SerializeField] private ScrollRect _habitsScrollView;
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _clearCheckmarksButton;
        [Header("Data")] 
        [SerializeField] private bool _saveDataInEditor = true;

        private HabitsData _currentData;
        
        public void Initialize(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            Setup();
            Hide();
        }

        private void OnEnable()
        {
            _addButton.onClick.AddListener(OnAddHabit);
            _clearCheckmarksButton.onClick.AddListener(OnClearCheckmarks);
        }

        private void OnDisable()
        {
            _addButton.onClick.RemoveListener(OnAddHabit);
            _clearCheckmarksButton.onClick.RemoveListener(OnClearCheckmarks);
        }

        private void OnAddHabit()
        {
            HabitData habit = new HabitData();
            _currentData.Habits.Add(habit);
            CreateHabitObject(habit);
            
            Save();
        }

        private void OnClearCheckmarks()
        {
            foreach (HabitData habit in _currentData.Habits)
            {
                habit.Monday = false;
                habit.Tuesday = false;
                habit.Wednesday = false;
                habit.Thursday = false;
                habit.Friday = false;
                habit.Saturday = false;
                habit.Sunday = false;
            }
            
            foreach (Habit habit in _habitsScrollView.content.GetComponentsInChildren<Habit>())
            {
                habit.ClearCheckmarks();
            }

            Save();
        }
        
        private void Setup()
        {
            _currentData = _saveLoadService.LoadData<HabitsData>(DATA_KEY);
            _currentData ??= GetBasicHabitsData();
            
            SetupHabits(_currentData);
        }


        public void UpdateData()
        {
            Save();
        }

        public void DeleteHabit(HabitData habitData, GameObject habitObject)
        {
            _currentData.Habits.Remove(habitData);
            Destroy(habitObject);
            Save();
        }

        private void SetupHabits(HabitsData habitsData)
        {
            DestroyChildObjects(_habitsScrollView.content);

            foreach (HabitData habit in habitsData.Habits)
            {
                CreateHabitObject(habit);
            }
        }
        
        private void CreateHabitObject(HabitData data)
        {
            Habit habit = Instantiate(_habitPrefab, _habitsScrollView.content);

            habit.Initialize(data, deleter: this, dataUpdater: this);
        }
        
        private HabitsData GetBasicHabitsData()
        {
            HabitsData data = new HabitsData
            {
                Habits = new List<HabitData> { new HabitData() }
            };
            
            return data;
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

        public interface IHabitDeleter
        {
            public void DeleteHabit(HabitData habitData, GameObject habitObject);
        }
        
        [Serializable]
        public class HabitsData
        {
            public List<HabitData> Habits = new();
        }

        [Serializable]
        public class HabitData
        {
            public string Name = "My new habit...";
            public string Id = Guid.NewGuid().ToString();
            public bool Monday = false;
            public bool Tuesday = false;
            public bool Wednesday = false;
            public bool Thursday = false;
            public bool Friday = false;
            public bool Saturday = false;
            public bool Sunday = false;
        }
    }
}
