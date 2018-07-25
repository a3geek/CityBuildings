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
        }

        public const string PropData = "_data";
        public const string PropWindowTex = "_windowTex";

        [SerializeField]
        private Vector2 height = new Vector2(10f, 20f);
        [SerializeField]
        private float baseHeight = 2.5f;
        [SerializeField]
        private Material material = null;

        private WindowTexture winTex = null;
        private ComputeBuffer buffer = null;
        private List<Build> builds = new List<Build>();


        public void Init(WindowTexture windowTexture)
        {
            this.CreateBuilds();

            this.winTex = windowTexture;

            this.buffer = new ComputeBuffer(this.builds.Count, Marshal.SizeOf(typeof(Build)), ComputeBufferType.Default);
            this.buffer.SetData(this.builds.ToArray());
        }

        private void OnRenderObject()
        {
            if(this.builds.Count < 0)
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
                var sec = sections[i];

                var randX = Random.Range(0.3f, 0.7f);
                var randZ = Random.Range(0.3f, 0.7f);

                this.builds.Add(new Build()
                {
                    center = sec.Center,
                    size = new Vector3(sec.Size.x * randX, Random.Range(this.height.x, this.height.y), sec.Size.z * randZ),
                    baseSize = new Vector3(
                        sec.Size.x * (randX < 0.5f ? 0.5f : randX + 0.2f),
                        this.baseHeight,
                        sec.Size.z * (randZ < 0.5f ? 0.5f : randZ + 0.2f)
                    )
                });
            }
        }
    }
}
