using UnityEngine;

namespace TodoBoard
{
    public interface ITaskReplacer
    {
        void OnDragStart();
        void OnDragComplete();
        void OnDragTask(TaskItem dragTask, Vector3 dragPosition);
        bool IsDragging { get; }
    }
}
