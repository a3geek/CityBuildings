using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Creators
{
    using Structs;
    using Utilities;
    using Random = UnityEngine.Random;

    [Serializable]
    public class MainRoadroller
    {
        public List<Road> Roads { get; } = new List<Road>();
        public List<Rect> Rects { get; } = new List<Rect>();

        [SerializeField]
        private float roadWidth = 16f;
        [SerializeField]
        private float rate = 0.1f;


        private Dictionary<Road, Color> colors = new Dictionary<Road, Color>();
        public void DrawGizmos()
        {
            var roads = this.Roads;
            var color = Gizmos.color;

            for(var i = 0; i < roads.Count; i++)
            {
                var road = roads[i];

                if(this.colors.ContainsKey(road) == false)
                {
                    this.colors[road] = new Color(Random.value, Random.value, Random.value, 1f);
                }

                Gizmos.color = this.colors[road];

                var center = (road.From + road.To) * 0.5f;
                var diff = road.To - road.From;

                var size = diff.ToVector3() + (diff.x != 0f ? Vector3.forward * road.Width : Vector3.right * road.Width);
                Gizmos.DrawCube(center.ToVector3(), size);
            }

            Gizmos.color = color;
        }

        public void Create(Vector2 field, Vector4 section, float interval)
        {
            var max = new Vector2(section.y, section.w);
            var pos = Vector2.zero;
            var counter = 0;

            var horizontal = new List<float>();
            var vertical = new List<float>();

            while(counter < 2)
            {
                (pos - field)
                    .EachFunc((i, v) => v < field[i] - max[i] ? v : field[i])
                    .EachAction((i, v) =>
                    {
                        if(pos[i] < 0f)
                        {
                            return;
                        }

                        var isLast = v == field[i];
                        if(v != -field[i] && isLast == false && Random.value >= this.rate)
                        {
                            return;
                        }

                        this.GetList(i, horizontal, vertical).Add(v);
                        this.StepPos(ref pos, max);

                        if(isLast == true)
                        {
                            counter++;
                            pos[i] = -1f;
                        }
                    });

                this.StepPos(ref pos, max);
            }

            this.Create(field, horizontal, vertical, interval);
        }

        private void Create(Vector2 field, List<float> horizontal, List<float> vertical, float interval)
        {
            var min = -1f * field;
            var max = field;

            var preHori = min.x;

            for(var i = 0; i < horizontal.Count; i++)
            {
                var h = horizontal[i];
                var preVert = min.y;

                vertical.ForEach(v =>
                {
                    this.Rects.Add(new Rect(preHori, preVert, h - preHori, v - preVert));

                    this.Roads.Add(new Road(new Vector2(preHori, preVert), new Vector2(h, preVert), this.roadWidth, interval));
                    this.Roads.Add(new Road(new Vector2(preHori, preVert), new Vector2(preHori, v), this.roadWidth, interval));

                    if(i == horizontal.Count - 1)
                    {
                        var from = new Vector2(max.x, preVert);
                        var to = new Vector2(max.x, v);
                        this.Roads.Add(new Road(from, to, this.roadWidth, interval));
                    }

                    preVert = v;
                });

                this.Roads.Add(new Road(new Vector2(preHori, max.y), new Vector2(h, max.y), this.roadWidth, interval));
                preHori = h;
            }
        }

        private void StepPos(ref Vector2 pos, Vector2 max)
        {
            pos = pos.EachFunc(max, (v1, v2) => v1 + (v1 < 0f ? 0f : v2));
        }

        private List<float> GetList(int id, List<float> l0, List<float> l1)
        {
            return id == 0 ? l0 : l1;
        }
    }
}
