using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace NightCity.Components
{
    using Utilities;
    using Creators;
    using Build = Creators.BuildingsCreator.Build;
    
    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Sky Scraper")]
    public class Skyscraper : MonoBehaviour
    {
        public const string PropData = "_data";
        public const string PropRandSeeds = "_randSeeds";
        public const string PropWindowNumberX = "_windowNumberX";
        public const string PropWindowNumberY = "_windowNumberY";
        public const string PropWindowTex = "_windowTex";

        [SerializeField]
        private BuildingsCreator buildings = new BuildingsCreator();
        [SerializeField]
        private Material material = null;

        private WindowTexture winTex = null;
        private ComputeBuffer dataBuffer = null;
        private ComputeBuffer seedsBuffer = null;
        

        public void Init(WindowTexture windowTexture)
        {
            this.winTex = windowTexture;

            List<Build> builds;
            List<uint> seeds;

            this.buildings.CreateBuilds(out builds, out seeds);
            if(builds.Count <= 0)
            {
                return;
            }
            
            this.dataBuffer = new ComputeBuffer(builds.Count, Marshal.SizeOf(typeof(Build)), ComputeBufferType.Default);
            this.dataBuffer.SetData(builds.ToArray());

            this.seedsBuffer = new ComputeBuffer(seeds.Count, Marshal.SizeOf(typeof(uint)), ComputeBufferType.Default);
            this.seedsBuffer.SetData(seeds.ToArray());
        }

        private void OnRenderObject()
        {
            if(this.dataBuffer.count <= 0)
            {
                return;
            }

            this.material.SetPass(0);

            this.material.SetBuffer(PropData, this.dataBuffer);
            this.material.SetBuffer(PropRandSeeds, this.seedsBuffer);
            this.material.SetTexture(PropWindowTex, this.winTex.Tex);

            var windowNumber = this.winTex.WindowNumber;
            this.material.SetInt(PropWindowNumberX, windowNumber.x);
            this.material.SetInt(PropWindowNumberY, windowNumber.y);

            Graphics.DrawProcedural(MeshTopology.Points, 3, this.dataBuffer.count);
        }

        private void OnDestroy()
        {
            this.dataBuffer?.Release();
            this.seedsBuffer?.Release();
        }
    }
}
