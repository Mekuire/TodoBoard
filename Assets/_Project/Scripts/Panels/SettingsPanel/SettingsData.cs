using System;
using UnityEngine;

namespace TodoBoard
{
    [Serializable]
    public class SettingsData
    {
        public string language;
        public bool alwaysOnTop = true;
        public int fpsFocused = 12;
        public int fpsUnFocused = 5;
        public string inputOverride = "";
        public HSVColor bgColor = HSVColor.white;
    }
}