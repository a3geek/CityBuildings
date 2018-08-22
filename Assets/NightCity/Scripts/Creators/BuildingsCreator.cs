using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NightCity.Creators
{
    using Utilities;
    using Components;

    [Serializable]
    public class BuildingsCreator
    {
        [Serializable]
        public struct Build
        {
            public Vector3 center;
            public Vector3 size;
            public Vector3 uvRange;
            public uint buildType;
        }
        
        [SerializeField]
        private Vector2 height = new Vector2(15f, 40f);
        [SerializeField]
        private Vector2 width = new Vector2(35f, 20f);
        [SerializeField]
        private Vector2 depth = new Vector2(35f, 20f);
        [SerializeField]
        private Vector2 rate = new Vector2(0.75f, 0.95f);
        [SerializeField]
        private int windowSize = 1;
        [SerializeField]
        private float specialRate = 0.05f;
        [SerializeField]
        private Vector2 specialHeight = new Vector2(50f, 75f);


        public void CreateBuilds(out List<Build> builds, out List<uint> seeds)
        {
            var sections = CityArea.Instance.Sections;
            builds = new List<Build>();
            seeds = new List<uint>();

            for(var i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                this.CreateBuild(section, ref builds, ref seeds);
            }
        }

        private void CreateBuild(CityArea.Section section, ref List<Build> builds, ref List<uint> seeds)
        {
            var size = section.Size;
            var center = section.Center;

            if(size.x <= this.width.x && size.z <= this.depth.x)
            {
                this.AddSeeds(ref seeds, 2);
                builds.Add(this.CreateBuild(center, size.x, size.z, this.height, this.rate));
                return;
            }

            var count = new Vector2Int(Mathf.RoundToInt(size.x / this.width.y), Mathf.RoundToInt(size.z / this.depth.y));
            var division = new Vector2(1f / count.x, 1f / count.y);
            var bl = section.BottomLeft.XZ();
            var div = size.XZ() * division;

            for(var i = 0; i < count.x; i++)
            {
                for(var j = 0; j < count.y; j++)
                {
                    var cen = bl + 0.5f * div + div * new Vector2(i, j);
                    var field = new Vector2(size.x * division.x, size.z * division.y);

                    this.AddSeeds(ref seeds, 2);
                    builds.Add(this.CreateBuild(cen.ToVector3(center.y), field.x, field.y, this.height, this.rate));
                }
            }
        }

        private Build CreateBuild(Vector3 center, float width, float depth, Vector2 height, Vector2 rate)
        {
            var wid = rate.Rand() * width;
            var dep = rate.Rand() * depth;
            var hei = Random.value < this.specialRate ? this.specialHeight.Rand() : height.Rand();

            var size = new Vector3(wid, hei, dep);
            size = size - size.Surplus(this.windowSize);

            return new Build()
            {
                center = center,
                size = size,
                uvRange = size / this.windowSize,
                buildType = Random.value < 0.5f ? 0u : 1u
            };
        }

        private void AddSeeds(ref List<uint> seeds, int count)
        {
            for(var i = 0; i < count; i++)
            {
                seeds.Add((uint)(Random.value * uint.MaxValue));
            }
        }
    }
}
