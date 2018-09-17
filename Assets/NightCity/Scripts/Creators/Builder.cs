using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NightCity.Creators
{
    using Structs;
    using Utilities;
    using Components;

    [Serializable]
    public class Builder
    {
        public List<ProceduralData> Procedurals { get; } = new List<ProceduralData>();
        public List<BuildingGeomData> Geoms { get; } = new List<BuildingGeomData>();
        public List<BuildingFragData> Frags { get; } = new List<BuildingFragData>();
        public List<DecorationData> Decos { get; } = new List<DecorationData>();
        public List<uint> Seeds { get; } = new List<uint>();
        
        [SerializeField]
        private Vector2 height = new Vector2(15f, 40f);
        [SerializeField]
        private Vector2 width = new Vector2(35f, 20f);
        [SerializeField]
        private Vector2 depth = new Vector2(35f, 20f);
        [SerializeField]
        private Vector2 mainRate = new Vector2(0.5f, 0.75f);
        [SerializeField]
        private Vector2 subRate = new Vector2(0.75f, 0.95f);
        [SerializeField]
        private int windowSize = 1;
        [SerializeField]
        private List<Color> colors = new List<Color>();
        [SerializeField]
        private float specialRate = 0.05f;
        [SerializeField]
        private Vector2 specialHeight = new Vector2(50f, 75f);

        private uint index = 0;


        public void CreateBuilds(Section[,] sections)
        {
            for(var i = 0; i < sections.GetLength(0); i++)
            {
                for(var j = 0; j < sections.GetLength(1); j++)
                {
                    this.CreateBuild(sections[i, j]);
                }
            }
        }

        private void CreateBuild(Section section)
        {
            var size = section.Size;
            var center = section.Center;

            if(size.x <= this.width.x && size.z <= this.depth.x)
            {
                this.AddFrags(1);
                this.AddSeeds(Skyscraper.VertexCount);
                this.Geoms.Add(this.CreateBuild(center, size.x, size.z));
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
                    this.AddSeeds(Skyscraper.VertexCount);
                    this.Geoms.AddRange(this.CreateBuilds(cen.ToVector3(center.y), field.x, field.y));
                }
            }
        }

        private BuildingGeomData CreateBuild(Vector3 center, float width, float depth)
        {
            return this.CreateBuilds(center, width, depth)[0];
        }

        private List<BuildingGeomData> CreateBuilds(Vector3 center, float width, float depth)
        {
            var data = new List<BuildingGeomData>();

            var widRate = this.mainRate.Rand();
            var depRate = this.mainRate.Rand();
            var isSpecial = Random.value < this.specialRate;
            var heiRate = isSpecial ? this.specialHeight.Rand() : this.height.Rand();

            var wid = widRate * width;
            var dep = depRate * depth;
            var hei = heiRate;

            var size = new Vector3(wid, hei, dep);
            size = size - size.Surplus(this.windowSize);

            var buildType = Random.value < 0.5f ? 0u : 1u;
            data.Add(new BuildingGeomData()
            {
                Center = center,
                Size = size,
                UvRange = size / this.windowSize,
                BuildType = buildType
            });

            if(buildType == 1u)
            {
                this.AddProcedurals(Skyscraper.VertexCount, 1u);
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
                size = (field * new Vector2(this.subRate.Rand(), this.subRate.Rand())).ToVector3(
                    (isSpecial == true ? Random.Range(this.specialHeight.x, heiRate) : Random.Range(height.x, heiRate)) * 0.9f
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
