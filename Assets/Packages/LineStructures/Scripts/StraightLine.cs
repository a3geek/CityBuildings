using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures
{
    using Components;

    [Serializable]
    public struct StraightLine : ILine
    {
        public Vector2 this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0: return this.p;
                    case 1: return this.p + this.t * this.dir;
                    default: throw new IndexOutOfRangeException("Invalid StraightLine index");
                }
            }
            set
            {
                switch(index)
                {
                    case 0:
                        this.p = value;
                        break;
                    case 1:
                        this.dir = value;
                        this.t = value.magnitude;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid StraightLine index");
                }
            }
        }

        public LineType LineType
        {
            get { return LineType.Straight; }
        }
        public Vector2 dir
        {
            get { return this.direction; }
            set { this.direction = value.normalized; }
        }
        public Vector2 normal
        {
            get { return this.dir.Normal(); }
        }
        public ILine perpendicularBisector
        {
            get { return this.GetPerpendicularBisector(); }
        }

        public Vector2 p;
        public float t;

        private Vector2 direction;


        public StraightLine(Vector2 referencePoint, Vector2 direction) : this(referencePoint, direction, direction.magnitude)
        {
        }

        public StraightLine(Vector2 referencePoint, Vector2 direction, float parametric)
        {
            this.p = referencePoint;
            this.direction = direction.normalized;
            this.t = parametric;
        }
        
        public StraightLine GetPerpendicularBisector()
        {
            return this.GetPerpendicularBisector(this.t);
        }

        public StraightLine GetPerpendicularBisector(float parametric)
        {
            return new StraightLine(this.p + 0.5f * this.t * this.dir, parametric * this.normal);
        }

        public void DrawGizmos(float y = 0f)
        {
            this.DrawGizmos(y, false);
        }

        public void DrawGizmos(float y, bool centering = false)
        {
            var dir = this.t * this.dir;
            var half = 0.5f * dir;

            var p0 = this.p - (centering == true ? half : Vector2.zero);
            var p1 = this.p + (centering == true ? half : dir);

            Gizmos.DrawLine(new Vector3(p0.x, y, p0.y), new Vector3(p1.x, y, p1.y));
        }
    }
}
