using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NightCity.Structs
{
    using Creators;
    using Utilities;
    using Structs;

    using Random = UnityEngine.Random;

    [Serializable]
    public struct Car
    {
        [SerializeField]
        private Vector2 pos;
        [SerializeField]
        private Vector2 dir;

        [SerializeField]
        private int roadID;
        [SerializeField]
        private float progress;
        [SerializeField]
        private int isForward;


        public Car(int roadID)
        {
            this.pos = this.dir = Vector2.zero;

            this.roadID = roadID;
            this.progress = Random.value;
            this.isForward = Random.value < 0.5f ? 1 : -1;
        }
        
        public void Update(CityArea city, float speed, float offset)
        {
            var road = city.Roads[this.roadID];

            if(this.GetProgress(road) >= 1f)
            {
                this.SetNextRoad(road, city);
            }
            else
            {
                this.progress = Mathf.Min(road.Magnitude, this.progress + speed);
            }

            this.dir = this.isForward > 0 ? road.Direction : -1f * road.Direction;

            var normal = this.dir.Normal() * offset;
            this.pos = (this.isForward > 0 ? road.From : road.To) + this.dir * this.progress + normal;
        }

        public float GetProgress(Road road)
        {
            return this.progress / road.Magnitude;
        }
        
        private void SetNextRoad(Road road, CityArea city)
        {
            var ids = RoadPointer.GetRoadsID(this.isForward == 1 ? road.ToPointID : road.FromPointID)
                .Where(id => id != road.Id).ToList();

            var nextID = ids.Rand();
            var next = city.Roads[nextID];

            this.roadID = nextID;
            this.progress = 0f;
            this.isForward = (this.isForward == 1 ? road.To : road.From) == next.From ? 1 : -1;
        }
    }
}
