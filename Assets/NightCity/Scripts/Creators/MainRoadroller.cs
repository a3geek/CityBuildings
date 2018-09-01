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
        [Serializable]
        public class Section
        {
            public Road[] Roads
            {
                get { return new Road[] { this.Top, this.Right, this.Bottom, this.Left }; }
            }

            public Rect Rect = new Rect();

            public Road Top = new Road();
            public Road Right = new Road();
            public Road Bottom = new Road();
            public Road Left = new Road();
        }

        public List<Road> Roads { get; } = new List<Road>();
        public List<Rect> Rects { get; } = new List<Rect>();
        public Section[,] Sections { get; private set; } = new Section[0, 0];

        [SerializeField]
        private float roadWidth = 16f;
        [SerializeField]
        private float rate = 0.1f;

        public float delay = 1.5f;
        public float radius = 10f;
        private float timer = 0f;
        private int index = 0;
        private int vert = 0;
        private int hor = 0;
        private List<float> Verticals = new List<float>();
        private List<float> Horizontals = new List<float>();
        private Vector2 field = Vector2.zero;

        public void DrawGizmos()
        {
            if(this.Sections.LongLength <= 0)
            {
                return;
            }

            this.timer += Time.deltaTime;
            if(this.timer > this.delay)
            {
                this.index = (this.index + 1) % (this.vert * this.hor);
                this.timer = 0f;
            }

            Debug.Log(this.vert + " _ " + this.hor + " __ " + this.Sections.LongLength);

            var shift = this.index / this.vert;
            var section = this.Sections[shift, this.index - this.vert * shift];

            section?.Top.Line.DrawGizmos();
            section?.Right.Line.DrawGizmos();
            section?.Bottom.Line.DrawGizmos();
            section?.Left.Line.DrawGizmos();
            
            for(var i = 0; i < this.Horizontals.Count; i++)
            {
                Gizmos.DrawSphere(new Vector3(this.Horizontals[i], 0f, -this.field.y), this.radius);
            }

            for(var i = 0; i < this.Verticals.Count; i++)
            {
                Gizmos.DrawSphere(new Vector3(-this.field.x, 0f, this.Verticals[i]), this.radius);
            }
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
                    .EachAction((i, v) => pos[i] >= 0f, (i, v) =>
                    {
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

            this.Sections = new Section[horizontal.Count - 1, vertical.Count - 1];
            this.vert = vertical.Count - 1;
            this.hor = horizontal.Count - 1;
            this.Verticals = vertical;
            this.Horizontals = horizontal;
            this.field = field;

            for(var i = 1; i < horizontal.Count; i++)
            {
                var h = horizontal[i];
                var preVert = vertical[0];
                Road hor;

                for(var j = 1; j < vertical.Count; j++)
                {
                    var v = vertical[j];
                    var refPoint = new Vector2(preHori, preVert);

                    hor = new Road(refPoint, new Vector2(h, preVert), w, interval);
                    var vert = new Road(refPoint, new Vector2(preHori, v), w, interval);

                    //this.Rects.Add(new Rect(preHori + 0.5f * w, preVert + 0.5f * w, h - preHori - w, v - preVert - w));
                    var sec = this.Sections[i - 1, j - 1] ?? new Section();
                    this.Sections[i - 1, j - 1] = sec;

                    sec.Bottom = hor;
                    sec.Left = vert;

                    Debug.Log(this.Sections[Mathf.Max(0, i - 2), j - 1] + " __ " + Mathf.Max(0, i - 2) + " _ " + (j - 1));
                    this.Sections[Mathf.Max(0, i - 2), j - 1].Right = vert;
                    this.Sections[i - 1, Mathf.Max(0, j - 2)].Top = hor;

                    //sec.AddRoad(hor).AddRoad(vert).AddRoad()


                    this.Roads.Add(hor);
                    this.Roads.Add(vert);

                    if(i == horizontal.Count - 1)
                    {
                        vert = new Road(new Vector2(max.x, preVert), new Vector2(max.x, v), w, interval);

                        //this.Sections[i - 1, j - 1].Right = vert;
                        this.Roads.Add(vert);
                    }

                    preVert = v;
                }

                hor = new Road(new Vector2(preHori, max.y), new Vector2(h, max.y), w, interval);

                //this.Sections[i - 1, vertical.Count - 1].Top = hor;
                this.Roads.Add(hor);
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
