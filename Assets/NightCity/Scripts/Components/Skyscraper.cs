﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace NightCity.Components
{
    using Utilities;

    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Sky Scraper")]
    public class Skyscraper : MonoBehaviour
    {
        [Serializable]
        private struct Build
        {
            public Vector3 center;
            public Vector3 size;
            public Vector3 uvStep;
            public uint randSeed;
        }

        public const string PropData = "_data";
        public const string PropWindowTex = "_windowTex";
        
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

        [Space]
        [SerializeField]
        private Material material = null;

        private WindowTexture winTex = null;
        private ComputeBuffer buffer = null;
        private List<Build> builds = new List<Build>();


        public void Init(WindowTexture windowTexture)
        {
            this.winTex = windowTexture;
            this.windowSize = Mathf.IsPowerOfTwo(this.windowSize) ? this.windowSize : Mathf.NextPowerOfTwo(this.windowSize);

            this.CreateBuilds();
            if(this.builds.Count <= 0)
            {
                return;
            }

            this.buffer = new ComputeBuffer(this.builds.Count, Marshal.SizeOf(typeof(Build)), ComputeBufferType.Default);
            this.buffer.SetData(this.builds.ToArray());
        }

        private void OnRenderObject()
        {
            if(this.builds.Count <= 0)
            {
                return;
            }

            this.material.SetPass(0);

            this.material.SetBuffer(PropData, this.buffer);
            this.material.SetTexture(PropWindowTex, this.winTex.Tex);

            Graphics.DrawProcedural(MeshTopology.Points, 1, this.buffer.count);
        }

        private void OnDestroy()
        {
            this.buffer?.Release();
        }

        private void CreateBuilds()
        {
            var sections = CityArea.Instance.Sections;

            for(var i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                this.CreateBuild(section);
            }
        }

        private Build CreateBuild(Vector3 center, float width, float depth, Vector2 height, Vector2 rate)
        {
            var wid = rate.Rand() * width;
            var dep = rate.Rand() * depth;
            var hei = height.Rand();

            var size = new Vector3(wid, hei, dep);
            size = size - size.Surplus(this.windowSize);

            return new Build()
            {
                center = center,
                size = size,
                uvStep = size / this.windowSize,
                randSeed = (uint)(Random.value * uint.MaxValue)
            };
        }
        
        private void CreateBuild(CityArea.Section section)
        {
            var size = section.Size;
            var center = section.Center;
            
            if(size.x <= this.width.x && size.z <= this.depth.x)
            {
                this.builds.Add(this.CreateBuild(center, size.x, size.z, this.height, this.rate));
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

                    this.builds.Add(this.CreateBuild(cen.ToVector3(center.y), field.x, field.y, this.height, this.rate));
                }
            }
        }
    }
}
