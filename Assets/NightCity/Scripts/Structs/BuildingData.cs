using System;
using UnityEngine;

namespace NightCity.Structs
{
    [Serializable]
    public struct BuildingGeomData
    {
        public Vector3 center;
        public Vector3 size;
        public Vector3 uvRange;
        public uint buildType;
        public int index;
    }

    [Serializable]
    public struct BuildingFragData
    {
        public Color colors;
    }
}
