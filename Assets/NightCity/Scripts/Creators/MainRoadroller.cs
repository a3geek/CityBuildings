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
            var max = field;
            var w = this.roadWidth;
            var preHori = horizontal[0];

            for(var i = 1; i < horizontal.Count; i++)
            {
                var h = horizontal[i];
                var preVert = vertical[0];

                for(var j = 1; j < vertical.Count; j++)
                {
                    var v = vertical[j];
                    var refPoint = new Vector2(preHori, preVert);
                    
                    this.Rects.Add(new Rect(preHori + 0.5f * w, preVert + 0.5f * w, h - preHori - w, v - preVert - w));

                    this.Roads.Add(new Road(refPoint, new Vector2(h, preVert), w, interval));
                    this.Roads.Add(new Road(refPoint, new Vector2(preHori, v), w, interval));

                    if(i == horizontal.Count - 1)
                    {
                        var from = new Vector2(max.x, preVert);
                        var to = new Vector2(max.x, v);
                        this.Roads.Add(new Road(from, to, w, interval));
                    }

                    preVert = v;
                }
                
                this.Roads.Add(new Road(new Vector2(preHori, max.y), new Vector2(h, max.y), w, interval));
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
