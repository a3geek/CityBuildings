using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Structs
{
    [Serializable]
    public struct SimpleRoad
    {
#pragma warning disable 0414
        public Vector2 From;
        public Vector2 To;
        public float FromOffset;
        public float ToOffset;
        public float Width;
        public float Interval;
        public float Magnitude;
        public Vector2 Direction;
#pragma warning restore 0414
    }

    [Serializable]
    public struct Road
    {
        private static int ID = 0;

        public Vector2 OffsetFrom => this.from + this.direction * this.fromOffset;
        public Vector2 OffsetTo => this.to - this.direction * this.toOffset;
        public float OffsetMagnitude => this.magnitude - this.fromOffset - this.toOffset;
        
        public Vector2 From => this.from;
        public Vector2 To => this.to;
        public float FromOffset => this.fromOffset;
        public float ToOffset => this.toOffset;
        public float Width => this.width;
        public float Interval => this.interval;

        public int Id => this.id;
        public int FromPointID => this.fromPointID;
        public int ToPointID => this.toPointID;
        public float Magnitude => this.magnitude;
        public Vector2 Direction => this.direction;

        [SerializeField]
        private Vector2 from;
        [SerializeField]
        private Vector2 to;
        [SerializeField]
        private float fromOffset;
        [SerializeField]
        private float toOffset;
        [SerializeField]
        private float width;
        [SerializeField]
        private float interval;

        [NonSerialized]
        private int id;
        [NonSerialized]
        private int fromPointID;
        [NonSerialized]
        private int toPointID;
        [NonSerialized]
        private float magnitude;
        [NonSerialized]
        private Vector2 direction;


        public Road(Vector2 from, Vector2 to, float fromOffset, float toOffset, float width, float interval)
        {
            this.id = ID;
            this.from = from;
            this.to = to;
            this.fromOffset = fromOffset;
            this.toOffset = toOffset;
            this.width = width;
            this.interval = interval;

            var dir = (to - from);
            this.direction = dir.normalized;
            this.magnitude = dir.magnitude;

            this.fromPointID = RoadPointer.AddPoint(from, ID);
            this.toPointID = RoadPointer.AddPoint(to, ID);

            ID++;
        }

        public static implicit operator SimpleRoad(Road road)
        {
            var mag = Vector2.Distance(road.from + road.direction * road.fromOffset, road.to - road.direction * road.toOffset);

            return new SimpleRoad()
            {
                From = road.from,
                To = road.to,
                FromOffset = road.fromOffset,
                ToOffset = road.toOffset,
                Width = road.width,
                Interval = road.interval,
                Magnitude = mag,
                Direction = road.direction
            };
        }
    }
}
