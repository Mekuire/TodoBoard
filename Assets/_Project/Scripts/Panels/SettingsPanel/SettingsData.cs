using System;

namespace TodoBoard
{
    [Serializable]
    public class SettingsData
    {
        public string language;
        public bool alwaysOnTop = true;
        public int fpsFocused = 5;
        public int fpsUnFocused = 1;
        public string inputOverride = "";
        public float clickVolume = 10f;
        public float pomodoroAlarm = 10f;
    }
}