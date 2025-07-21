using System;
using System.Collections.Generic;

namespace TodoBoard
{
    [Serializable]
    public class ToDoData
    {
        public List<TaskListData> TaskLists;
    }
}