using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace NightCity.Components
{
    public static class VectorUtil
    {
        public static Vector3 Surplus(this Vector3 v3, float v)
        {
            return new Vector3(v3.x % v, v3.y % v, v3.z % v);
        }
    }

    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Sky Scraper")]
    public class Skyscraper : MonoBehaviour
    {
        [Serializable]
        private struct Build
        {
            public Vector3 center;
            public Vector3 size;
            public Vector3 baseSize;
            public Vector3 uvStep;
            public uint randSeed;
        }

        public const string PropData = "_data";
        public const string PropWindowTex = "_windowTex";

        [SerializeField]
        private Vector2 height = new Vector2(10f, 20f);
        [SerializeField]
        private int windowSize = 8;
        [SerializeField]
        private float baseHeight = 2.5f;
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

                var size = new Vector3(
                    section.Size.x * Random.Range(0.3f, 0.7f),
                    Random.Range(this.height.x, this.height.y),
                    section.Size.z * Random.Range(0.3f, 0.7f)
                );
                size = size - size.Surplus(this.windowSize);
                
                this.builds.Add(new Build()
                {
                    center = section.Center,
                    size = size,
                    baseSize = size + new Vector3(
                        (section.Size.x - size.x) * 0.5f,
                        this.baseHeight - size.y,
                        (section.Size.z - size.z) * 0.5f
                    ),
                    uvStep = size / this.windowSize,
                    randSeed = (uint)(Random.value * uint.MaxValue)
                });
            }
        }
    }
}
