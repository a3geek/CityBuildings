using System;
using UnityEngine;

namespace CityBuildings.Structs
{
    [Serializable]
    public struct ProceduralData
    {
        public uint Id;
        public uint Index;
        public uint Verts;
        public uint Range;
    }

    [Serializable]
    public struct DecorationData
    {
        public Vector3 Center;
        public Vector3 Size;
        public uint BuildType;
        public float Height;
    }

    [Serializable]
    public struct BuildingGeomData
    {
        public Vector3 Center;
        public Vector3 Size;
        public Vector3 UvRange;
        public uint BuildType;


        public bool IsContains(Vector2 pos)
        {
            var min = this.Center - this.Size * 0.5f;
            var max = this.Center + this.Size * 0.5f;

            return (pos.x >= min.x && pos.x <= max.x) &&
                (pos.y >= min.z && pos.y <= max.z);
        }

        public bool IsIn(Vector2 center, Vector2 size)
        {
            var min = center - size * 0.5f;
            var max = center + size * 0.5f;

            return this.IsContains(min) && this.IsContains(max) &&
                this.IsContains(new Vector2(min.x, max.y)) && this.IsContains(new Vector2(max.x, min.y));
        }
    }

    [Serializable]
    public struct BuildingFragData
    {
        public Color Color;
    }
}
