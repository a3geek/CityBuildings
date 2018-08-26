using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NightCity.Components
{
    using Components;
    using LineStructures;
    using Structs;
    using Utilities;

    [Serializable]
    public class CityArea
    {
        public List<Section> Sections { get; } = new List<Section>();
        //public List<Road> roads
        //{
        //    get { return this.vertical.Concat(this.horizontal).ToList(); }
        //}

        public List<Road> VerticalRoad { get; } = new List<Road>();
        public List<Road> HorizontalRoad { get; } = new List<Road>();
        public List<Road> Roads { get; } = new List<Road>();


        [SerializeField]
        private Vector2 field = new Vector2(1000f, 1000f);
        [SerializeField]
        private Vector2 sectionX = new Vector2(30f, 60f);
        [SerializeField]
        private Vector2 sectionY = new Vector2(30f, 60f);

        [Header("Road"), Space]
        [SerializeField]
        private float mainRoadWidth = 16f;
        [SerializeField]
        private float subRoadWidth = 4f;
        [SerializeField]
        private float mainRate = 0.1f;
        [SerializeField]
        private float interval = 1f;


        public void CreateAreas()
        {
            // First : main road.
            var max = new Vector2(this.sectionX.y, this.sectionY.y);
            var counter = 0;
            var pos = Vector2.zero;
            
            var horizontal = new List<float>();
            var vertical = new List<float>();

            Action posStep = () =>
            {
                pos = pos.EachFunc(max, (v1, v2) => v1 + (v1 < 0f ? 0f : v2));
            };

            var j = 0;
            var k = 0;
            do
            {
                (pos - this.field)
                    .EachFunc((i, v) => v < this.field[i] - max[i] ? v : this.field[i])
                    .EachAction((i, v) =>
                    {
                        if(pos[i] < 0f)
                        {
                            return;
                        }

                        var isLast = v == this.field[i];
                        if(v != -this.field[i] && isLast == false && Random.value >= this.mainRate)
                        {
                            return;
                        }

                        //var i2 = (i == 0 ? 1 : 0);
                        ////Vector2 from = Vector2.zero, to = Vector2.zero;
                        //var to = Vector2.zero;

                        ////from[i] = to[i] = v;
                        ////from[i2] = -this.field[i];
                        ////to[i2] = this.field[i];
                        //to[i] = v;
                        //to[i2] = this.field[i];

                        ////this.AddRoad(new Road(from, to, this.mainRoadWidth, this.interval),
                        ////    i == 0 ? horizontal : vertical,
                        ////    i == 0 ? vertical : horizontal
                        ////);

                        if(i == 0)
                        {
                            horizontal.Add(v);
                        }
                        else
                        {
                            vertical.Add(v);
                        }

                        posStep();
                        k++;

                        if(isLast == true)
                        {
                            counter++;
                            pos[i] = -1f;
                        }
                    });

                posStep();
            }
            while(counter < 2);

            this.MainRoad(horizontal, vertical);
            //this.Roads.AddRange(horizontal);
            //this.Roads.AddRange(vertical);
        }

        private List<Rect> MainRoad(List<float> horizontal, List<float> vertical)
        {
            var min = -1f * this.field;
            var max = this.field;

            var ph = min.x;
            var rects = new List<Rect>();

            for(var i = 0; i < horizontal.Count; i++)
            {
                var h = horizontal[i];
                var pv = min.y;

                vertical.ForEach(v =>
                {
                    rects.Add(new Rect(ph, pv, h - ph, v - pv));

                    this.Roads.Add(new Road(new Vector2(ph, pv), new Vector2(h, pv), this.mainRoadWidth, this.interval));
                    this.Roads.Add(new Road(new Vector2(ph, pv), new Vector2(ph, v), this.mainRoadWidth, this.interval));

                    if(i == horizontal.Count - 1)
                    {
                        var from = new Vector2(max.x, pv);
                        var to = new Vector2(max.x, v);
                        this.Roads.Add(new Road(from, to, this.mainRoadWidth, this.interval));
                    }

                    pv = v;
                });

                this.Roads.Add(new Road(new Vector2(ph, max.y), new Vector2(h, max.y), this.mainRoadWidth, this.interval));
                ph = h;
            }

            return rects;
        }


        //private void AddRoad(Road road, List<Road> group, List<Road> others)
        //{
        //    var roads = new List<Road>();

        //    for(var i = 0; i < others.Count; i++)
        //    {
        //        var o = others[i];
        //        var line = road.Line;

        //        if(Lines.IsCrossing(line, o.Line) == false)
        //        {
        //            continue;
        //        }
        
        //        var cross = Lines.GetIntersection(road.Line, o.Line);
        //        if(line.p0 == cross || line.p1 == cross)
        //        {
        //            continue;
        //        }

        //        var p1 = line.p1;
        //        roads.Add(new Road(line.p0, cross, road.Width, road.Interval));
        //        roads.Add(new Road(cross, p1, road.Width, road.Interval));

        //        road = roads[roads.Count - 1];
        //    }

        //    group.AddRange(roads.Count <= 0 ? new List<Road>() { road } : roads);
        //}
    }
}
