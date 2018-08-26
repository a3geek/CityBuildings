using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures
{
    using Components;

    public static partial class Lines
    {
        public const float EPS = Vector2.kEpsilon;

        
        public static float GetDistance(StraightLine self, Vector2 p)
        {
            var v = p - self.p;
            // 本当は this.Cross(this.dir, v) / this.dir.magnitude だが、DirectionのMagnitudeは1.
            return Mathf.Abs(Vector2Util.Cross(self.dir, v));
        }

        public static float GetDistance(HalfStraightLine self, Vector2 p)
        {
            return Vector2.Dot(self.dir, p - self.p) < EPS ?
                Vector2.Distance(self.p, p) :
                Mathf.Abs(Vector2Util.Cross(self.dir, p - self.p)) / self.dir.magnitude;
        }

        public static float GetDistance(LineSegment self, Vector2 p)
        {
            if(Vector2.Dot(self.dir, p - self.p0) < EPS)
            {
                return Vector2.Distance(self.p0, p);
            }
            if(Vector2.Dot(-self.dir, p - self.p1) < EPS)
            {
                return Vector2.Distance(self.p1, p);
            }

            return Mathf.Abs(Vector2Util.Cross(self.dir, p - self.p0)) / self.dir.magnitude;
        }

        private static float GetIntersectionParametric(Vector2 p, Vector2 dir, Vector2 p1, Vector2 dir1)
        {
            var n = dir1.Normal();
            return Vector2.Dot(n, p1 - p) / Vector2.Dot(n, dir);
        }
    }
}
