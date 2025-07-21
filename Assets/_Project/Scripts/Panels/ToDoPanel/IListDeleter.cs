using UnityEngine;

namespace TodoBoard
{
    public interface IListDeleter
    {
        public void DeleteList(TaskListData taskList, GameObject list);
    }
}