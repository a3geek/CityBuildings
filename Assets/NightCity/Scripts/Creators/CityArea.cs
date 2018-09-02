using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Creators
{
    using Structs;
    using Utilities;

    using Random = UnityEngine.Random;

    [Serializable]
    public class CityArea
    {
        private class SplitPoint
        {
            public float HalfWidth => 0.5f * this.Width;
            public float Point { get; }
            public float Width { get; }


            public SplitPoint(float point, float width)
            {
                this.Point = point;
                this.Width = width;
            }
        }

        public Section[,] Sections { get; private set; } = new Section[0, 0];
        public List<Road> Roads { get; } = new List<Road>();

        [SerializeField]
        private Vector2 field = new Vector2(1000f, 1000f);
        [SerializeField]
        private Vector2 sectionX = new Vector2(30f, 60f);
        [SerializeField]
        private Vector2 sectionY = new Vector2(30f, 60f);

        [Header("About Road"), Space]
        [SerializeField]
        private MainRoadParams main = new MainRoadParams();
        [SerializeField]
        private SubRoadParams sub = new SubRoadParams();
        [SerializeField]
        private float interval = 1f;


        public void Create()
        {
            var max = new Vector2(this.sectionX.y, this.sectionY.y);
            var pos = Vector2.zero;
            var step = Vector2.zero;
            var counter = 0;

            var pointsX = new List<SplitPoint>();
            var pointsY = new List<SplitPoint>();

            while(counter < 2)
            {
                (pos - this.field)
                    .EachAction((i, p) => pos[i] >= 0f, (i, p) =>
                    {
                        var isFrame = (p <= -this.field[i] == true || p >= this.field[i] == true);
                        var isMain = Random.value < this.main.Rate && isFrame == false;

                        p += 0.5f * (isMain == true ? max[i] : 0f);
                        p = p < this.field[i] - max[i] ? p : this.field[i];

                        var isLast = (p >= this.field[i]);
                        isMain = (isMain || isFrame || isLast);
                        (i == 0 ? pointsX : pointsY).Add(new SplitPoint(p, isMain == true ? this.main.Width : this.sub.Width));

                        step[i] = (isMain == true ? max[i] : 0f);
                        if(isLast == true)
                        {
                            counter++;
                            pos[i] = -1f;
                        }
                    });

                step += new Vector2(this.sectionX.Rand(), this.sectionY.Rand());
                pos = pos.EachFunc(step, (v1, v2) => v1 + (v1 < 0f ? 0f : v2));
            }

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

                    var hor = new Road(refPoint, new Vector2(px.Point, preY.Point), preY.Width, this.interval, false);
                    var vert = new Road(refPoint, new Vector2(preX.Point, py.Point), preX.Width, this.interval, true);

                    var edge = new Vector2(px.Point - px.HalfWidth, py.Point - py.HalfWidth);
                    var preEdge = new Vector2(preX.Point + preX.HalfWidth, preY.Point + preY.HalfWidth);
                    this.Sections[i - 1, j - 1].Set(
                        0.5f * (edge + preEdge),
                        new Vector2(
                            edge.x - preEdge.x,
                            edge.y - preEdge.y
                        )
                    );

                    this.Roads.Add(hor);
                    this.Roads.Add(vert);

                    if(i == pointsX.Count - 1)
                    {
                        vert = new Road(new Vector2(max.x, preY.Point), new Vector2(max.x, py.Point), px.Width, this.interval, true);
                        this.Roads.Add(vert);
                    }

                    preY = py;
                }

                this.Roads.Add(new Road(new Vector2(preX.Point, max.y), new Vector2(px.Point, max.y), preY.Width, this.interval, false));
                preX = px;
            }
        }
    }
}
