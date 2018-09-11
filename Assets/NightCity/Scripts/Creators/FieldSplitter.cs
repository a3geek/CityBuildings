using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Creators
{
    using Structs;
    using Utilities;
    
    using Random = UnityEngine.Random;

    [Serializable]
    public class FieldSplitter
    {
        public List<SplitPoint> PointsX { get; } = new List<SplitPoint>();
        public List<SplitPoint> PointsY { get; } = new List<SplitPoint>();

        [SerializeField]
        private Vector2 sectionX = new Vector2(120f, 180f);
        [SerializeField]
        private Vector2 sectionY = new Vector2(60f, 90f);

        [Header("About Road"), Space]
        [SerializeField]
        private MainRoadParams main = new MainRoadParams();
        [SerializeField]
        private SubRoadParams sub = new SubRoadParams();


        public void Create(Vector2 field)
        {
            var max = new Vector2(this.sectionX.y, this.sectionY.y);
            var pos = Vector2.zero;
            var step = Vector2.zero;
            var counter = 0;
            
            while(counter < 2)
            {
                (pos - field)
                    .EachAction((i, p) => pos[i] >= 0f, (i, p) =>
                    {
                        var isFrame = (p <= -field[i] == true || p >= field[i] == true);
                        var isMain = Random.value < this.main.Rate && isFrame == false;

                        p += 0.5f * (isMain == true ? max[i] : 0f);
                        p = p < field[i] - max[i] ? p : field[i];

                        var isLast = (p >= field[i]);
                        isMain = (isMain || isFrame || isLast);

                        var sp = new SplitPoint(p, isMain == true ? this.main.Width : this.sub.Width);
                        (i == 0 ? this.PointsX : this.PointsY).Add(sp);

                        step[i] = (isMain == true ? max[i] : 0f);
                        if(isLast == true)
                        {
                            counter++;
                            pos[i] = -1f;
                        }
                    });

                step += new Vector2(this.sectionX.Rand(), this.sectionY.Rand());
                pos = pos.EachFunc(step, (v1, Vector2) => v1 + (v1 < 0f ? 0f : Vector2));
            }
        }
    }
}
