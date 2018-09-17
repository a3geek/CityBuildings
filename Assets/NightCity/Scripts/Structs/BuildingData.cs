using System;
using UnityEngine;

namespace NightCity.Structs
{
    [Serializable]
    public struct ProceduralData
    {
        public uint id;
        public uint index;
        public uint verts;
        public uint range;
    }

    [Serializable]
    public struct BuildingGeomData
    {
        public Vector3 center;
        public Vector3 size;
        public Vector3 uvRange;
        public uint buildType;


        public bool IsContains(Vector2 pos)
        {
            var min = this.center - this.size * 0.5f;
            var max = this.center + this.size * 0.5f;

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
        public Color colors;
    }
}
