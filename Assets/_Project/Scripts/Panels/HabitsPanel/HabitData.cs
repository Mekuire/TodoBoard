using System;

namespace TodoBoard
{
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
