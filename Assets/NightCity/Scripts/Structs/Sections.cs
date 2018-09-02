using System;
using UnityEngine;

namespace NightCity.Structs
{
    using Utilities;
    using LineStructures;

    [Serializable]
    public struct Section
    {
        public static float DefaultCenterY = 0f;
        public static float DefaultSizeY = 0f;

        public Vector3 HalfSize => 0.5f * this.Size;
        public Vector3 BottomLeft => this.Center - this.HalfSize;
        public Vector3 TopRight => this.Center + this.HalfSize;
        public Vector3 TopLeft
        {
            get
            {
                var half = this.HalfSize;
                return this.Center + new Vector3(-half.x, 0f, half.z);
            }
        }
        public Vector3 BottomRight
        {
            get
            {
                var half = this.HalfSize;
                return this.Center + new Vector3(half.x, 0f, -half.z);
            }
        }

        public Vector3 Center => this.center;
        public Vector3 Size => this.size;

        [SerializeField]
        private Vector3 center;
        [SerializeField]
        private Vector3 size;

        
        public void Set(Vector2 center, Vector2 size)
        {
            this.center = new Vector3(center.x, DefaultCenterY, center.y);
            this.size = new Vector3(size.x, DefaultSizeY, size.y);
        }
    }

    [Serializable]
    public struct Road
    {
        public Vector2 From
        {
            get { return this.from; }
            set
            {
                this.from = value;

                this.line.p0 = value;
                this.normal = this.line.normal;
            }
        }
        public Vector2 To
        {
            get { return this.to; }
            set
            {
                this.to = value;

                this.line.p1 = value;
                this.normal = this.line.normal;
            }
        }
        public LineSegment Line => this.line;
        public Vector2 Normal => this.normal;
        public float Width => this.width;
        public float Interval => this.interval;
        public bool IsVertical { get; }

        [SerializeField]
        private Vector2 from;
        [SerializeField]
        private Vector2 to;
        [SerializeField]
        private Vector2 normal;
        [SerializeField]
        private float width;
        [SerializeField]
        private float interval;

        private LineSegment line;


        public Road(Vector2 from, Vector2 to, float width, float interval, bool isVertical)
        {
            this.from = from;
            this.to = to;
            this.width = width;
            this.interval = interval;
            this.IsVertical = isVertical;

            this.line = new LineSegment(from, to);
            this.normal = this.line.normal;
        }
    }
}
