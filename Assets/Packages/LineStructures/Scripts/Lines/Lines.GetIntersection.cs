using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures
{
    using Components;
    using Dic = Dictionary<Components.LineType, Dictionary<Components.LineType, Func<Components.ILine, Components.ILine, Vector2>>>;

    public static partial class Lines
    {
        private static Dic DicForGetIntersectionByInterface = new Dic()
        {
            {
                LineType.Segment, new Dictionary<LineType, Func<ILine, ILine, Vector2>>()
                {
                    { LineType.Segment, (l1, l2) => { return GetIntersection((LineSegment)l1, (LineSegment)l2); } },
                    { LineType.Straight, (l1, l2) => { return GetIntersection((StraightLine)l2, (LineSegment)l1); } },
                    { LineType.HalfStraight, (l1, l2) => { return GetIntersection((HalfStraightLine)l2, (LineSegment)l1); } }
                }
            },
            {
                LineType.Straight, new Dictionary<LineType, Func<ILine, ILine, Vector2>>()
                {
                    { LineType.Segment, (l1, l2) => { return GetIntersection((StraightLine)l1, (LineSegment)l2); } },
                    { LineType.Straight, (l1, l2) => { return GetIntersection((StraightLine)l1, (StraightLine)l2); } },
                    { LineType.HalfStraight, (l1, l2) => { return GetIntersection((StraightLine)l1, (HalfStraightLine)l2); } }
                }
            },
            {
                LineType.HalfStraight, new Dictionary<LineType, Func<ILine, ILine, Vector2>>()
                {
                    { LineType.Segment, (l1, l2) => { return GetIntersection((HalfStraightLine)l1, (LineSegment)l2); } },
                    { LineType.Straight, (l1, l2) => { return GetIntersection((StraightLine)l2, (HalfStraightLine)l1); } },
                    { LineType.HalfStraight, (l1, l2) => { return GetIntersection((HalfStraightLine)l1, (HalfStraightLine)l2); } }
                }
            }
        };
        

        public static Vector2 GetIntersection(ILine self, ILine other)
        {
            return DicForGetIntersectionByInterface[self.LineType][other.LineType](self, other);
        }

        public static Vector2 GetIntersection(LineSegment self, LineSegment other)
        {
            return GetIntersectionWithLineSegment(self, other.p0, other.dir);
        }
        
        public static Vector2 GetIntersection(StraightLine self, StraightLine other)
        {
            var t = GetIntersectionParametric(self.p, self.dir, other.p, other.dir);
            return self.p + t * self.dir;
        }

        public static Vector2 GetIntersection(HalfStraightLine self, HalfStraightLine other)
        {
            var t = GetIntersectionParametric(self.p, self.dir, other.p, other.dir);
            return self.p + t * self.dir;
        }

        public static Vector2 GetIntersection(StraightLine self, LineSegment other)
        {
            return GetIntersectionWithLineSegment(other, self.p, self.dir);
        }

        public static Vector2 GetIntersection(StraightLine self, HalfStraightLine other)
        {
            var t = GetIntersectionParametric(self.p, self.dir, other.p, other.dir);
            return self.p + t * self.dir;
        }

        public static Vector2 GetIntersection(HalfStraightLine self, LineSegment other)
        {
            return GetIntersectionWithLineSegment(other, self.p, self.dir);
        }

        private static Vector2 GetIntersectionWithLineSegment(LineSegment self, Vector2 p, Vector2 dir)
        {
            var d1 = Mathf.Abs(Vector2Util.Cross(dir, self.p0 - p));
            var d2 = Mathf.Abs(Vector2Util.Cross(dir, self.p1 - p));
            var t = d1 / (d1 + d2);

            return self.p0 + t * self.dir;
        }
    }
}
