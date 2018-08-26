using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures
{
    using Components;

    [Serializable]
    public struct LineSegment : ILine
    {
        public Vector2 this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0: return this.p0;
                    case 1: return this.p1;
                    default: throw new IndexOutOfRangeException("Invalid LineSegment index");
                }
            }
            set
            {
                switch(index)
                {
                    case 0:
                        this.p0 = value;
                        break;
                    case 1:
                        this.p1 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid LineSegment index");
                }
            }
        }

        public LineType LineType
        {
            get { return LineType.Segment; }
        }
        public Vector2 center
        {
            get { return 0.5f * (this.p0 + this.p1); }
        }
        public Vector2 dir
        {
            get { return this.p1 - this.p0; }
        }
        public Vector2 normal
        {
            get { return this.dir.Normal(); }
        }
        public ILine perpendicularBisector
        {
            get { return this.GetPerpendicularBisector(); }
        }

        public Vector2 p0;
        public Vector2 p1;
        

        public LineSegment(Vector2 startPoint, Vector2 endPoint)
        {
            this.p0 = startPoint;
            this.p1 = endPoint;
        }
        
        public LineSegment GetPerpendicularBisector()
        {
            return new LineSegment(this.center, this.center + this.normal);
        }
        
        public void DrawGizmos(float y = 0f)
        {
            var p0 = new Vector3(this.p0.x, y, this.p0.y);
            var p1 = new Vector3(this.p1.x, y, this.p1.y);

            Gizmos.DrawLine(p0, p1);
        }
    }
}
