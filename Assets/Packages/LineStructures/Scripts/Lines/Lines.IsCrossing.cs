using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures
{
    using Components;
    using Dic = Dictionary<Components.LineType, Dictionary<Components.LineType, Func<Components.ILine, Components.ILine, bool>>>;

    public static partial class Lines
    {
        private static Dic DicForIsCrossingByInterface = new Dic()
        {
            {
                LineType.Segment, new Dictionary<LineType, Func<ILine, ILine, bool>>()
                {
                    { LineType.Segment, (l1, l2) => { return IsCrossing((LineSegment)l1, (LineSegment)l2); } },
                    { LineType.Straight, (l1, l2) => { return IsCrossing((StraightLine)l2, (LineSegment)l1); } },
                    { LineType.HalfStraight, (l1, l2) => { return IsCrossing((HalfStraightLine)l2, (LineSegment)l1); } }
                }
            },
            {
                LineType.Straight, new Dictionary<LineType, Func<ILine, ILine, bool>>()
                {
                    { LineType.Segment, (l1, l2) => { return IsCrossing((StraightLine)l1, (LineSegment)l2); } },
                    { LineType.Straight, (l1, l2) => { return IsCrossing((StraightLine)l1, (StraightLine)l2); } },
                    { LineType.HalfStraight, (l1, l2) => { return IsCrossing((StraightLine)l1, (HalfStraightLine)l2); } }
                }
            },
            {
                LineType.HalfStraight, new Dictionary<LineType, Func<ILine, ILine, bool>>()
                {
                    { LineType.Segment, (l1, l2) => { return IsCrossing((HalfStraightLine)l1, (LineSegment)l2); } },
                    { LineType.Straight, (l1, l2) => { return IsCrossing((StraightLine)l2, (HalfStraightLine)l1); } },
                    { LineType.HalfStraight, (l1, l2) => { return IsCrossing((HalfStraightLine)l1, (HalfStraightLine)l2); } }
                }
            }
        };


        public static bool IsCrossing(ILine self, ILine other)
        {
            return DicForIsCrossingByInterface[self.LineType][other.LineType](self, other);
        }

        public static bool IsCrossing(StraightLine self, StraightLine other)
        {
            // a × b == 0 ⇔ a // b
            return !(Mathf.Abs(Vector2Util.Cross(self.dir, other.dir)) < EPS);
        }

        public static bool IsCrossing(HalfStraightLine self, HalfStraightLine other)
        {
            return (
                Vector2Util.Cross(self.dir, other.p - self.p) * Vector2Util.Cross(self.dir, other.dir) < 0f &&
                GetIntersectionParametric(self.p, self.dir, other.p, other.dir) >= 0f
            );
        }

        public static bool IsCrossing(LineSegment self, LineSegment other)
        {
            return (
                Vector2Util.Cross(self.dir, other.p0 - self.p0) *
                Vector2Util.Cross(self.dir, other.p1 - self.p0) < EPS
            ) && (
                Vector2Util.Cross(other.dir, self.p0 - other.p0) *
                Vector2Util.Cross(other.dir, self.p1 - other.p0) < EPS
            );
        }

        public static bool IsCrossing(StraightLine self, HalfStraightLine other)
        {
            return Vector2Util.Cross(self.dir, other.p - self.p) * Vector2Util.Cross(self.dir, other.dir) < 0f;
        }

        public static bool IsCrossing(StraightLine self, LineSegment other)
        {
            return Vector2Util.Cross(self.dir, other.p0 - self.p) * Vector2Util.Cross(self.dir, other.p1 - self.p) < 0f;
        }

        public static bool IsCrossing(HalfStraightLine self, LineSegment other)
        {
            var v1 = other.p0 - self.p;
            var v2 = other.p1 - self.p;

            return (
                (Vector2.Dot(self.dir, v1) >= 0f || Vector2.Dot(self.dir, v2) >= 0f) &&
                Vector2Util.Cross(self.dir, v1) * Vector2Util.Cross(self.dir, v2) < 0f &&
                (GetIntersectionParametric(self.p, self.dir, other.p0, other.dir) >= 0f)
            );
        }
    }
}
