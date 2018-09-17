using System;
using UnityEngine;

namespace NightCity.Structs
{
    public struct SplitPoint
    {
        public float HalfWidth => 0.5f * this.Width;
        public float Point { get; }
        public float Width { get; }


        public SplitPoint(float point, float width)
        {
            this.Point = point;
            this.Width = width;
        }
    }
}
