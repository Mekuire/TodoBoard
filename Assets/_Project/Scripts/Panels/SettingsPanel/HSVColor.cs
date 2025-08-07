using UnityEngine;

namespace TodoBoard
{
    public readonly struct HSVColor
    {
        public HSVColor(float h, float s, float v)
        {
            this.h = h;
            this.s = s;
            this.v = v;
        }
        
        public readonly float h, s, v;
        public static readonly HSVColor white = new HSVColor(0, 0, 1);
        public static readonly HSVColor black = new HSVColor(0, 1, 0);
    }
}
