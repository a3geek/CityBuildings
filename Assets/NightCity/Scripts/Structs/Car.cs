using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Structs
{
    [Serializable]
    public struct Car
    {
        public int RoadID;
        public float Progress;
        public int Dir;


        public Vector2 GetPos(Road road)
        {
            var dir = road.Direction * (this.Dir > 0 ? 1 : -1);
            return (this.Dir > 0 ? road.From : road.To) + dir * this.Progress;
        }

        public float GetProgress(Road road)
        {
            return this.Progress / road.Magnitude;
        }
    }
}
