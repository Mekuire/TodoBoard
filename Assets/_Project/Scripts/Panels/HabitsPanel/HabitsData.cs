using System;
using System.Collections.Generic;

namespace TodoBoard
{
    [Serializable]
    public class HabitsData
    {
        public List<HabitData> Habits = new();
    }
}