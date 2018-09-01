using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Creators
{
    using Structs;
    using Utilities;
    using Random = UnityEngine.Random;

    [Serializable]
    public class SubRoadroller
    {
        public List<Section> Sections { get; } = new List<Section>();
        public List<Road> Roads = new List<Road>();

        [SerializeField]
        private float roadWidth = 4f;


        public void Create(Vector4 section, MainRoadroller main)
        {
            var rects = main.Rects;
            for(var i = 0; i < rects.Count; i++)
            {
                this.Create(section, rects[i], main.Roads);
            }
        }

        private void Create(Vector4 section, Rect field, List<Road> mains)
        {
            var secX = section.XY();
            var secY = section.ZW();
            
            if(field.size.x <= secX.y || field.size.y <= secY.y)
            {
                this.Sections.Add(new Section(field.center, field.size));
                return;
            }

            var dirIsX = field.size.x > field.size.y;
            this.Create(field,
                dirIsX == true ? Vector2.right : Vector2.up,
                dirIsX == true ? secX : secY,
                dirIsX == true ? secY : secX, mains
            );
        }

        private void Create(Rect field, Vector2 dir, Vector2 sec, Vector2 subSec, List<Road> mains)
        {
            var subDir = -1f * (dir - Vector2.one);
            
        }

        private void Create(Rect field, int index, Vector2 sec, Vector2 subSec, Road[] mains)
        {
            var subIndex = -1 * (index - 1);
            var min = field.min;
            var max = field.max;

            for(var w = min[index]; w < max[index];)
            {
                var stepW = sec.Rand();
                w = (w + stepW) >= (max[index] - sec.Average()) ? max[index] : w + stepW;

                for(var h = min[subIndex]; h < max[subIndex];)
                {
                    var stepH = subSec.Rand();
                    h = (h + stepH) >= (max[subIndex] - subSec.Average()) ? max[subIndex] : h + stepH;


                }
            }
        }
    }
}
