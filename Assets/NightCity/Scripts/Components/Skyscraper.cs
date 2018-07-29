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
        private float baseHeight = 2.5f;
        [SerializeField]
        private float widthPerWindow = 8f;
        [SerializeField]
        private float heightPerWindow = 8f;
        [SerializeField]
        private float depthPerWindow = 8f;
        [Space]
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

            var wpw = this.widthPerWindow;
            var hpw = this.heightPerWindow;
            var dpw = this.depthPerWindow;

            for(var i = 0; i < sections.Count; i++)
            {
                var sec = sections[i];
                
                var wid = sec.Size.x * Random.Range(0.3f, 0.7f);
                var dep = sec.Size.z * Random.Range(0.3f, 0.7f);
                
                wid = wid % wpw == 0f ? wid : wid + wpw - wid % wpw;
                dep = dep % dpw == 0f ? dep : dep + dpw - dep % dpw;

                var hei = Random.Range(this.height.x, this.height.y);
                hei = hei % hpw == 0f ? hei : hei + hpw - hei % hpw;
                
                this.builds.Add(new Build()
                {
                    center = sec.Center,
                    size = new Vector3(wid, hei, dep),
                    baseSize = new Vector3(
                        wid + (sec.Size.x - wid) * 0.5f,
                        this.baseHeight,
                        dep + (sec.Size.z - dep) * 0.5f
                    ),
                    uvStep = new Vector3(
                        wid / wpw,
                        hei / hpw,
                        dep / dpw
                    ),
                    randSeed = (uint)(Random.value * uint.MaxValue)
                });
            }
        }
    }
}
