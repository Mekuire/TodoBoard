using System;
using System.Collections.Generic;

namespace TodoBoard
{
    [Serializable]
    public class TaskListData
    {
        public string ListName = "My List";
        public string Id = Guid.NewGuid().ToString();
        public List<TaskData> Tasks;
    }
}