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

                    this.AddRoad(refPoint, new Vector2(px.Point, preY.Point), preX.Width, px.Width, preY.Width,index, new Vector2Int(
                        index.x, Mathf.Max(0, index.y - 1)
                    ));
                    this.AddRoad(refPoint, new Vector2(preX.Point, py.Point), preY.Width, py.Width, preX.Width, index, new Vector2Int(
                        Mathf.Max(0, index.x - 1), index.y
                    ));
                    
                    if(i == pointsX.Count - 1)
                    {
                        this.AddRoad(
                            new Vector2(max.x, preY.Point), new Vector2(max.x, py.Point), preY.Width, py.Width, px.Width,
                            index, index
                        );
                    }

                    preY = py;
                }

                this.AddRoad(
                    new Vector2(preX.Point, max.y), new Vector2(px.Point, max.y), preX.Width, px.Width, preY.Width,
                    new Vector2Int(i, pointsY.Count - 2), new Vector2Int(i, pointsY.Count - 2)
                );
                preX = px;
            }
        }

        private Road AddRoad(Vector2 from, Vector2 to, float fromOffset, float toOffset, float width, Vector2Int index1, Vector2Int index2)
        {
            var road = new Road(from, to, fromOffset, toOffset, width, this.interval);
            road.SectionIndex1 = index1;
            road.SectionIndex2 = index2;

            this.Roads.Add(road);
            return road;
        }
    }
}
