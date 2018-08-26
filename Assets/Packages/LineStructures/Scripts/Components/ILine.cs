using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures.Components
{
    [Serializable]
    public enum LineType
    {
        Segment = 1, Straight, HalfStraight
    }

    public interface ILine
    {
        Vector2 this[int index]
        {
            get; set;
        }

        LineType LineType
        {
            get;
        }
        Vector2 dir
        {
            get;
        }
        Vector2 normal
        {
            get;
        }
        ILine perpendicularBisector
        {
            get;
        }

        void DrawGizmos(float y = 0f);
    }
}
