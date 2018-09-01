using System;
using UnityEngine;

namespace NightCity.Utilities
{
    using LineStructures;
    using LineStructures.Components;

    public static class LinesExtends
    {
        public static bool IsOn(this LineSegment line, Vector2 p)
        {
            var dir = p - line.p0;
            return Mathf.Abs(Vector2Util.Cross(line.dir, dir)) < Lines.EPS;
        }
    }
}
