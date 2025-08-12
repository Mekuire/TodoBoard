using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TodoBoard
{
    public class HabitsPanel : Panel, IDataUpdater, IHabitDeleter
    {
        private const string DATA_KEY = "habits";

        [FormerlySerializedAs("_habitPrefab")] [SerializeField] private HabitItem habitItemPrefab;
        [SerializeField] private ScrollRect _habitsScrollView;
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _clearCheckmarksButton;
        [Header("Data")] 
        [SerializeField] private bool _saveDataInEditor = true;

        private HabitsData _currentData;
        private ISaveLoadService _saveLoadService;
        
        public override void Initialize(IServiceProvider provider)
        {
            _saveLoadService = provider.SaveLoadService;
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
            
            foreach (HabitItem habit in _habitsScrollView.content.GetComponentsInChildren<HabitItem>())
            {
                habit.ClearCheckmarks();
            }

            Save();
        }
        
        private void Setup()
        {
            _saveLoadService.LoadData<HabitsData>(DATA_KEY, out _currentData);
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
            HabitItem habitItem = Instantiate(habitItemPrefab, _habitsScrollView.content);

            habitItem.Initialize(data, deleter: this, dataUpdater: this);
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
    }
}
