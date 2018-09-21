using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

namespace CityBuildings.Managers
{
    using Creators;
    using Structs;

    [DisallowMultipleComponent]
    [AddComponentMenu("City Buildings/Managers/Roads Manager")]
    public class RoadsManager : MonoBehaviour
    {
        public const uint MaxVertexCount = 128u;
        public const uint PerVertex = 8u;
        public const uint MaxPointPerGeom = MaxVertexCount / PerVertex;

        public const string PropColor = "_Color";
        public const string PropLineColor = "_LineColor";
        public const string PropSize = "_Size";
        public const string PropBasicWidth = "_BasicWidth";
        public const string PropHeight = "_Height";
        public const string PropMaxPointPerGeom = "_MaxPointPerGeom";
        public const string PropGeomData = "_GeomData";
        public const string PropPower = "_Power";

        [SerializeField]
        private Color color = new Color(0.99f, 0.75f, 0.70f, 1f);
        [SerializeField]
        private Color line = Color.white;
        [SerializeField]
        private float size = 1f;
        [SerializeField]
        private float basicWidth = 24f;
        [SerializeField]
        private float height = 0f;
        [SerializeField]
        private float power = 1f;
        [SerializeField]
        private Material material = null;

        private ComputeBuffer geomBuffer = null;
        private int vertsCount = 1;


        public void Initialize(SkyscraperManager skyscraper)
        {
            var roads = skyscraper.CityArea.Roads.Values.Select(r => (SimpleRoad)r);

            this.geomBuffer = new ComputeBuffer(roads.Count(), Marshal.SizeOf(typeof(SimpleRoad)), ComputeBufferType.Default);
            this.geomBuffer.SetData(roads.ToArray());

            var step = skyscraper.CityArea.MaxDistance / skyscraper.CityArea.Interval;
            this.vertsCount = Mathf.CeilToInt(step / MaxPointPerGeom);
        }

        private void OnRenderObject()
        {
            if(this.geomBuffer == null)
            {
                return;
            }

            this.material.SetPass(0);

            this.material.SetColor(PropColor, this.color);
            this.material.SetColor(PropLineColor, this.line);
            this.material.SetFloat(PropSize, this.size);
            this.material.SetFloat(PropBasicWidth, this.basicWidth);
            this.material.SetFloat(PropHeight, this.height);
            this.material.SetBuffer(PropGeomData, this.geomBuffer);
            this.material.SetFloat(PropMaxPointPerGeom, MaxPointPerGeom);
            this.material.SetFloat(PropPower, this.power);

            Graphics.DrawProcedural(MeshTopology.Points, this.vertsCount, this.geomBuffer.count);
        }

        private void OnDestroy()
        {
            this.geomBuffer?.Release();
        }
    }
}
