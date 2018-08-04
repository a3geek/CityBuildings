using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace NightCity.Components
{
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
        private Vector2Int numPerSection = new Vector2Int(2, 2);
        [SerializeField]
        private Vector2 sizeRate = new Vector2(0.8f, 1f);
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

        private void CreateBuild(CityArea.Section section)
        {
            var step = section.Size.XZ() / this.numPerSection;
            Func<float> rater = () => Random.Range(this.sizeRate.x, this.sizeRate.y);

            for(var i = 0; i < this.numPerSection.x; i++)
            {
                for(var j = 0; j < this.numPerSection.y; j++)
                {
                    var baseSize = (step * new Vector2(rater(), rater())).ToVector3(this.baseHeight);

                    var height = Random.Range(this.height.x, this.height.y);
                    var size = (baseSize.XZ() - baseSize.XZ().Surplus(this.windowSize))
                        .ToVector3(height - height % this.windowSize);

                    var center = section.BottomLeft + (new Vector2(i, j) * step + 0.5f * baseSize.XZ() + new Vector2(
                        Random.Range(0f, step.x - baseSize.x),
                        Random.Range(0f, step.y - baseSize.z)
                    )).ToVector3();

                    this.builds.Add(new Build()
                    {
                        center = center,
                        size = size,
                        baseSize = baseSize,
                        uvStep = size / this.windowSize,
                        randSeed = (uint)(Random.value * uint.MaxValue)
                    });
                }
            }
        }
    }
}
