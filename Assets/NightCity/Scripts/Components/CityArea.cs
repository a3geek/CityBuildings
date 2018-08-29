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
                        
                        this.GetList(i, horizontal, vertical);
                        posStep();

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

        public List<float> GetList(int id, List<float> l0, List<float> l1)
        {
            return id == 0 ? l0 : l1;
        }
    }
}
