using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Structs
{
    [Serializable]
    public struct SimpleRoad
    {
#pragma warning disable 0414
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
#pragma warning restore 0414


        public SimpleRoad(Vector2 from, Vector2 to, float fromOffset, float toOffset, float width, float interval)
        {
            this.from = from;
            this.to = to;
            this.fromOffset = fromOffset;
            this.toOffset = toOffset;
            this.width = width;
            this.interval = interval;
        }
    }

    [Serializable]
    public struct Road
    {
        private static int ID = 0;

        public int Id => this.id;
        public int FromPointID => this.fromPointID;
        public int ToPointID => this.toPointID;
        public float Magnitude => this.magnitude;
        public Vector2 Direction => this.direction;

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
        
        public Vector2 From => this.from;
        public Vector2 To => this.to;
        public float FromOffset => this.fromOffset;
        public float ToOffset => this.toOffset;
        public float Width => this.width;
        public float Interval => this.interval;

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
            this.magnitude = dir.magnitude;
            this.direction = dir.normalized;

            this.fromPointID = RoadPointer.AddPoint(from, ID);
            this.toPointID = RoadPointer.AddPoint(to, ID);

            ID++;
        }

        public static implicit operator SimpleRoad(Road road)
        {
            return new SimpleRoad(road.from, road.to, road.fromOffset, road.toOffset, road.width, road.interval);
        }
    }
}
