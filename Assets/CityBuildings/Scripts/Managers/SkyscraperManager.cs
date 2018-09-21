using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

namespace CityBuildings.Managers
{
    using Structs;
    using Creators;
    
    [DisallowMultipleComponent]
    [AddComponentMenu("City Buildings/Managers/Sky Scraper")]
    public class SkyscraperManager : MonoBehaviour
    {
        public const int VertexCount = 3;

        public const string PropIsSceneCamera = "ON_RENDER_SCENE_VIEW";
        public const string PropProceduralData = "_ProceduralData";
        public const string PropGeomData = "_GeomData";
        public const string PropRandSeeds = "_RandSeeds";
        public const string PropFragData = "_FragData";
        public const string PropSeedStep = "_SeedStep";
        public const string PropWindowNumberX = "_WindowNumberX";
        public const string PropWindowNumberY = "_WindowNumberY";
        public const string PropWindowNumberZ = "_WindowNumberZ";
        public const string PropWindowTex = "_WindowTex";
        
        public Builder Builder => this.builder;
        public CityArea CityArea => this.cityarea;

        [SerializeField]
        private Builder builder = new Builder();
        [SerializeField]
        private CityArea cityarea = new CityArea();
        [SerializeField]
        private Material material = null;

        private WindowTextureManager winTex = null;
        private SkyManager sky = null;
        private ComputeBuffer proceduralBuffer = null;
        private ComputeBuffer geomsBuffer = null;
        private ComputeBuffer seedsBuffer = null;
        private ComputeBuffer fragsBuffer = null;


        public void Initialize(WindowTextureManager windowTexture, SkyManager sky)
        {
            this.winTex = windowTexture;
            this.sky = sky;
            
            this.cityarea.Create();
            this.Build();
        }

        public void Build(float specialRate = -1f)
        {
            this.builder.CreateBuilds(this.cityarea.Sections, specialRate < 0f ? this.builder.SpecialRate : specialRate);
            if(this.builder.Geoms.Count <= 0)
            {
                return;
            }

            this.SetBuffers();
        }

        private void SetBuffers()
        {
            this.proceduralBuffer?.Release();
            this.proceduralBuffer = this.CreateBuffer<ProceduralData>(this.builder.Procedurals.Count);
            this.proceduralBuffer.SetData(this.builder.Procedurals.ToArray());

            this.geomsBuffer?.Release();
            this.geomsBuffer = this.CreateBuffer<BuildingGeomData>(this.builder.Geoms.Count);
            this.geomsBuffer.SetData(this.builder.Geoms.ToArray());

            this.seedsBuffer?.Release();
            this.seedsBuffer = this.CreateBuffer<uint>(this.builder.Seeds.Count);
            this.seedsBuffer.SetData(this.builder.Seeds.ToArray());

            this.fragsBuffer?.Release();
            this.fragsBuffer = this.CreateBuffer<BuildingFragData>(this.builder.Frags.Count);
            this.fragsBuffer.SetData(this.builder.Frags.ToArray());
        }
        
        private void OnRenderObject()
        {
            if(this.geomsBuffer == null)
            {
                return;
            }

#if UNITY_EDITOR
            var cam = UnityEditor.SceneView.currentDrawingSceneView?.camera;
            if(cam != null && Camera.current == cam)
            {
                this.material.EnableKeyword(PropIsSceneCamera);
            }
            else
            {
                this.material.DisableKeyword(PropIsSceneCamera);
            }
#else
            this.material.DisableKeyword(PropIsSceneCamera);
#endif

            this.material.SetPass(0);

            this.material.SetBuffer(PropProceduralData, this.proceduralBuffer);
            this.material.SetBuffer(PropGeomData, this.geomsBuffer);
            this.material.SetBuffer(PropRandSeeds, this.seedsBuffer);
            this.material.SetBuffer(PropFragData, this.fragsBuffer);
            this.material.SetTexture(PropWindowTex, this.winTex[this.sky.Current]);

            var windowNumber = this.winTex.WindowNumber;
            this.material.SetInt(PropWindowNumberX, windowNumber.x);
            this.material.SetInt(PropWindowNumberY, windowNumber.y);
            this.material.SetInt(PropSeedStep, VertexCount);

            Graphics.DrawProcedural(MeshTopology.Points, VertexCount, this.proceduralBuffer.count);
        }

        private void OnDestroy()
        {
            this.proceduralBuffer?.Release();
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
