using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Creators
{
    using Structs;
    
    [Serializable]
    public class CityArea
    {
        public Section[,] Sections { get; private set; } = new Section[0, 0];
        public List<Road> Roads { get; } = new List<Road>();
        public Road MaxDistance { get; private set; }

        [SerializeField]
        private Vector2 field = new Vector2(1000f, 1000f);
        [SerializeField, Space]
        private FieldSplitter splitter = new FieldSplitter();
        [SerializeField, Space]
        private float interval = 20f;


        public void Create()
        {
            List<SplitPoint> pointsX, pointsY;

            this.splitter.Create(this.field, out pointsX, out pointsY);
            this.Create(pointsX, pointsY);
        }

        private void Create(List<SplitPoint> pointsX, List<SplitPoint> pointsY)
        {
            var max = this.field;
            var preX = pointsX[0];

            this.Sections = new Section[pointsX.Count - 1, pointsY.Count - 1];
            for(var i = 1; i < pointsX.Count; i++)
            {
                var px = pointsX[i];
                var preY = pointsY[0];

                for(var j = 1; j < pointsY.Count; j++)
                {
                    var py = pointsY[j];
                    var refPoint = new Vector2(preX.Point, preY.Point);
                    
                    var edge = new Vector2(px.Point - px.HalfWidth, py.Point - py.HalfWidth);
                    var preEdge = new Vector2(preX.Point + preX.HalfWidth, preY.Point + preY.HalfWidth);
                    this.Sections[i - 1, j - 1].Set(
                        0.5f * (edge + preEdge),
                        new Vector2(
                            edge.x - preEdge.x,
                            edge.y - preEdge.y
                        )
                    );

                    this.AddRoad(refPoint, new Vector2(px.Point, preY.Point), preX.Width, px.Width, preY.Width);
                    this.AddRoad(refPoint, new Vector2(preX.Point, py.Point), preY.Width, py.Width, preX.Width);
                    
                    if(i == pointsX.Count - 1)
                    {
                        this.AddRoad(
                            new Vector2(max.x, preY.Point), new Vector2(max.x, py.Point), preY.Width, py.Width, px.Width
                        );
                    }

                    preY = py;
                }

                this.AddRoad(
                    new Vector2(preX.Point, max.y), new Vector2(px.Point, max.y), preX.Width, px.Width, preY.Width
                );
                preX = px;
            }
        }

        private Road AddRoad(Vector2 from, Vector2 to, float fromOffset, float toOffset, float width)
        {
            var road = new Road(from, to, fromOffset, toOffset, width, this.interval);
            this.Roads.Add(road);

            return road;
        }
    }
}
