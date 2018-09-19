using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuildings.Creators
{
    using Structs;
    
    [Serializable]
    public class CityArea
    {
        public Section[,] Sections { get; private set; } = new Section[0, 0];
        public Dictionary<int, Road> Roads { get; } = new Dictionary<int, Road>();
        public float MaxDistance { get; private set; } = 0f;
        public float Interval => this.interval;
        public Vector2 Field => this.field;
        public Vector2 FieldCenter => Vector2.zero;

        [SerializeField]
        private Vector2 field = new Vector2(1000f, 1000f);
        [SerializeField, Space]
        private FieldSplitter splitter = new FieldSplitter();
        [SerializeField, Space]
        private float interval = 20f;


        public void Create()
        {
            this.splitter.Create(this.field);
            this.Create(this.splitter.PointsX, this.splitter.PointsY);
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
                    var index = new Vector2Int(i - 1, j - 1);
                    
                    var edge = new Vector2(px.Point - px.HalfWidth, py.Point - py.HalfWidth);
                    var preEdge = new Vector2(preX.Point + preX.HalfWidth, preY.Point + preY.HalfWidth);
                    this.Sections[index.x, index.y].Set(
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

            this.MaxDistance = Mathf.Sqrt(this.MaxDistance);
        }

        private int AddRoad(Vector2 from, Vector2 to, float fromOffset, float toOffset, float width)
        {
            var mag = (to - from).sqrMagnitude;
            this.MaxDistance = Mathf.Max(this.MaxDistance, mag);

            var road = new Road(from, to, fromOffset, toOffset, width, this.interval);
            this.Roads[road.Id] = road;

            return road.Id;
        }
    }
}
