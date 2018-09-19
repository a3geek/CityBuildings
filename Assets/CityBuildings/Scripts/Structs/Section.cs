using System;
using UnityEngine;

namespace CityBuildings.Structs
{
    [Serializable]
    public struct Section
    {
        public static float DefaultCenterY = 0f;
        public static float DefaultSizeY = 0f;

        public Vector3 HalfSize => 0.5f * this.Size;
        public Vector3 BottomLeft => this.Center - this.HalfSize;
        public Vector3 TopRight => this.Center + this.HalfSize;
        public Vector3 TopLeft => this.Center + new Vector3(-this.HalfSize.x, 0f, this.HalfSize.z);
        public Vector3 BottomRight => this.Center + new Vector3(this.HalfSize.x, 0f, -this.HalfSize.z);

        public Vector3 Center => this.center;
        public Vector3 Size => this.size;

        [SerializeField]
        private Vector3 center;
        [SerializeField]
        private Vector3 size;

        
        public void Set(Vector2 center, Vector2 size)
        {
            this.center = new Vector3(center.x, DefaultCenterY, center.y);
            this.size = new Vector3(size.x, DefaultSizeY, size.y);
        }
    }
}
