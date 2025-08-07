using System;
using UnityEngine;

namespace TodoBoard
{
    [Serializable]
    public class SettingsData
    {
        public string language;
        public bool alwaysOnTop = true;
        public int fpsFocused = 5;
        public int fpsUnFocused = 5;
        public string inputOverride = "";
        public HSVColor bgColor = HSVColor.white;
        //public float clickVolume = 10f;
        //public float pomodoroAlarm = 10f;
    }
}