using UnityEngine;

namespace TodoBoard
{
    public interface ITaskDeleter
    {
        public void DeleteTask(TaskData taskData, GameObject task);
    }
}

