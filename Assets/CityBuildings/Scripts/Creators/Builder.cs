using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CityBuildings.Creators
{
    using Structs;
    using Utilities;
    using Managers;

    [Serializable]
    public class Builder
    {
        public List<ProceduralData> Procedurals { get; } = new List<ProceduralData>();
        public List<BuildingGeomData> Geoms { get; } = new List<BuildingGeomData>();
        public List<BuildingFragData> Frags { get; } = new List<BuildingFragData>();
        public List<DecorationData> Decos { get; } = new List<DecorationData>();
        public List<uint> Seeds { get; } = new List<uint>();

        public float SpecialRate => this.specialRate;
        
        [SerializeField]
        private Vector2 height = new Vector2(15f, 40f);
        [SerializeField]
        private Vector2 width = new Vector2(35f, 20f);
        [SerializeField]
        private Vector2 depth = new Vector2(35f, 20f);
        [SerializeField]
        private Vector2 cylinderMainRate = new Vector2(0.8f, 0.95f);
        [SerializeField]
        private Vector2 rectangularMainRate = new Vector2(0.4f, 0.6f);
        [SerializeField]
        private Vector2 rectangularSubRate = new Vector2(0.7f, 0.95f);
        [SerializeField]
        private int windowSize = 1;
        [SerializeField]
        private List<Color> colors = new List<Color>();
        [SerializeField]
        private float specialRate = 0.05f;
        [SerializeField]
        private Vector2 specialHeight = new Vector2(50f, 75f);
        [SerializeField]
        private float decoHeight = 5f;
        [SerializeField]
        private float roundDecoHeight = 3f;

        private uint index = 0;


        public void CreateBuilds(Section[,] sections)
        {
            this.CreateBuilds(sections, this.specialRate);
        }

        public void CreateBuilds(Section[,] sections, float specialRate)
        {
            this.Procedurals.Clear();
            this.Geoms.Clear();
            this.Frags.Clear();
            this.Decos.Clear();
            this.Seeds.Clear();
            this.index = 0;

            for(var i = 0; i < sections.GetLength(0); i++)
            {
                for(var j = 0; j < sections.GetLength(1); j++)
                {
                    this.CreateBuild(sections[i, j], specialRate);
                }
            }
        }

        private void CreateBuild(Section section, float specialRate)
        {
            var size = section.Size;
            var center = section.Center;

            if(size.x <= this.width.x && size.z <= this.depth.x)
            {
                this.AddFrags(1);
                this.AddSeeds(SkyscraperManager.VertexCount);
                this.Geoms.Add(this.CreateBuild(center, size.x, size.z, specialRate));
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

                    this.AddFrags(1);
                    this.AddSeeds(SkyscraperManager.VertexCount);
                    this.Geoms.AddRange(this.CreateBuilds(cen.ToVector3(center.y), field.x, field.y, specialRate));
                }
            }
        }

        private BuildingGeomData CreateBuild(Vector3 center, float width, float depth, float specialRate)
        {
            return this.CreateBuilds(center, width, depth, specialRate)[0];
        }

        private List<BuildingGeomData> CreateBuilds(Vector3 center, float width, float depth, float specialRate)
        {
            var data = new List<BuildingGeomData>();
            var buildType = Random.value < 0.5f ? 0u : 1u;

            var widRate = buildType == 0u ? this.rectangularMainRate.Rand() : this.cylinderMainRate.Rand();
            var depRate = buildType == 0u ? this.rectangularMainRate.Rand() : this.cylinderMainRate.Rand();
            var isSpecial = Random.value < specialRate;
            var heiRate = isSpecial ? this.specialHeight.Rand() : this.height.Rand();

            var wid = widRate * width;
            var dep = depRate * depth;
            var hei = heiRate;

            var size = new Vector3(wid, hei, dep);
            size = size - size.Surplus(this.windowSize);

            data.Add(new BuildingGeomData()
            {
                Center = center,
                Size = size,
                UvRange = size / this.windowSize,
                BuildType = buildType
            });

            if(isSpecial == true)
            {
                this.Decos.Add(new DecorationData()
                {
                    Center = center,
                    Size = size,
                    BuildType = buildType,
                    Height = buildType == 0u ? this.decoHeight : this.roundDecoHeight
                });
            }

            if(buildType == 1u)
            {
                this.AddProcedurals(SkyscraperManager.VertexCount, 1u);
                return data;
            }
            if(isSpecial == true)
            {
                data.Add(new BuildingGeomData()
                {
                    Center = center,
                    Size = size,
                    UvRange = Vector3.zero,
                    BuildType = 2u
                });
            }

            var field = (new Vector2(width, depth)) * 0.5f;
            var count = (uint)Random.Range(0, 3);
            for(var i = 0u; i < count; i++)
            {
                size = (field * new Vector2(this.rectangularSubRate.Rand(), this.rectangularSubRate.Rand())).ToVector3(
                    (isSpecial == true ? Random.Range(this.specialHeight.x, heiRate) * 0.9f : this.height.Rand())
                );
                size = size - size.Surplus(this.windowSize);

                var dir = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
                var pos = center.XZ() + size.XZ() * 0.5f * dir;

                if(data.IsContains(d => d.IsIn(pos, size.XZ())) == true)
                {
                    count--;
                    i--;
                    continue;
                }
                
                data.Add(new BuildingGeomData()
                {
                    Center = pos.ToVector3(center.y),
                    Size = size,
                    UvRange = size / this.windowSize,
                    BuildType = buildType
                });
            }
            
            this.AddProcedurals(1u, 1u + count + (isSpecial == true ? 1u : 0u));
            return data;
        }

        private void AddProcedurals(uint verts, uint range)
        {
            this.Procedurals.Add(new ProceduralData()
            {
                Id = (uint)this.Procedurals.Count,
                Index = this.index,
                Verts = verts,
                Range = range
            });

            this.index += range;
        }

        private void AddFrags(int count)
        {
            for(var i = 0; i < count; i++)
            {
                this.Frags.Add(new BuildingFragData() { Color = this.colors[Random.Range(0, this.colors.Count)] });
            }
        }

        private void AddSeeds(int count)
        {
            for(var i = 0; i < count; i++)
            {
                this.Seeds.Add((uint)(Random.value * uint.MaxValue));
            }
        }
    }
}
