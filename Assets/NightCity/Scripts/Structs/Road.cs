using System;
using UnityEngine;

namespace NightCity.Structs
{
    [Serializable]
    public struct Road
    {
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
            this.from = from;
            this.to = to;
            this.fromOffset = fromOffset;
            this.toOffset = toOffset;
            this.width = width;
            this.interval = interval;
        }
    }
}
