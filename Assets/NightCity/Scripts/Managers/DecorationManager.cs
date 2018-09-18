using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace NightCity.Managers
{
    using Structs;
    using Creators;

    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Managers/Decoration Manager")]
    public class DecorationManager : MonoBehaviour
    {
        public const string PropData = "_data";
        public const string PropTriangleSize = "_Radius";

        [SerializeField]
        private Material material = null;
        [SerializeField]
        private float radius = 0.5f;

        private ComputeBuffer buffer = null;


        public void Init(SkyscraperManager skyscraper)
        {
            var decos = skyscraper.Builder.Decos;

            this.buffer = new ComputeBuffer(decos.Count, Marshal.SizeOf(typeof(DecorationData)), ComputeBufferType.Default);
            this.buffer.SetData(decos.ToArray());
        }

        private void OnRenderObject()
        {
            if(this.buffer == null)
            {
                return;
            }

            this.material.SetPass(0);
            this.material.SetBuffer(PropData, this.buffer);
            this.material.SetFloat(PropTriangleSize, this.radius);

            Graphics.DrawProcedural(MeshTopology.Points, 4, this.buffer.count);
        }

        private void OnDestroy()
        {
            this.buffer?.Release();
        }
    }
}
