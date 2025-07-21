using UnityEngine;

namespace TodoBoard
{
    public interface IHabitDeleter
    {
        public void DeleteHabit(HabitData habitData, GameObject habitObject);
    }
}