using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace NightCity.Components
{
    using Structs;
    using Creators;
    
    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Sky Scraper")]
    public class Skyscraper : MonoBehaviour
    {
        public const string PropData = "_geomData";
        public const string PropRandSeeds = "_randSeeds";
        public const string PropFragData = "_fragData";
        public const string PropWindowNumberX = "_windowNumberX";
        public const string PropWindowNumberY = "_windowNumberY";
        public const string PropWindowTex = "_windowTex";

        [SerializeField]
        private Builder builder = new Builder();
        [SerializeField]
        private CityArea cityarea = new CityArea();
        [SerializeField]
        private Material material = null;

        private WindowTexture winTex = null;
        private ComputeBuffer geomsBuffer = null;
        private ComputeBuffer seedsBuffer = null;
        private ComputeBuffer fragsBuffer = null;


        public void Init(WindowTexture windowTexture)
        {
            this.winTex = windowTexture;
            
            this.cityarea.CreateAreas();
            this.builder.CreateBuilds(this.cityarea.Sections);

            if(this.builder.Geoms.Count <= 0)
            {
                return;
            }

            this.geomsBuffer = this.CreateBuffer<BuildingGeomData>(this.builder.Geoms.Count);
            this.geomsBuffer.SetData(this.builder.Geoms.ToArray());

            this.seedsBuffer = this.CreateBuffer<uint>(this.builder.Seeds.Count);
            this.seedsBuffer.SetData(this.builder.Seeds.ToArray());

            this.fragsBuffer = this.CreateBuffer<BuildingFragData>(this.builder.Frags.Count);
            this.fragsBuffer.SetData(this.builder.Frags.ToArray());
        }
        
        private void OnRenderObject()
        {
            if(this.geomsBuffer == null)
            {
                return;
            }

            this.material.SetPass(0);

            this.material.SetBuffer(PropData, this.geomsBuffer);
            this.material.SetBuffer(PropRandSeeds, this.seedsBuffer);
            this.material.SetBuffer(PropFragData, this.fragsBuffer);
            this.material.SetTexture(PropWindowTex, this.winTex.Tex);

            var windowNumber = this.winTex.WindowNumber;
            this.material.SetInt(PropWindowNumberX, windowNumber.x);
            this.material.SetInt(PropWindowNumberY, windowNumber.y);

            Graphics.DrawProcedural(MeshTopology.Points, 3, this.geomsBuffer.count);
        }

        private void OnDestroy()
        {
            this.geomsBuffer?.Release();
            this.seedsBuffer?.Release();
            this.fragsBuffer?.Release();
        }

        private ComputeBuffer CreateBuffer<T>(int count)
        {
            return new ComputeBuffer(count, Marshal.SizeOf(typeof(T)), ComputeBufferType.Default);
        }
    }
}
