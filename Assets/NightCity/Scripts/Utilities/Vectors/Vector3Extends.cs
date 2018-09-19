using UnityEngine;

namespace CityBuildings.Utilities
{
    public static class Vector3Extends
    {
        public static Vector3 Surplus(this Vector3 v3, float v)
        {
            return new Vector3(v3.x % v, v3.y % v, v3.z % v);
        }

        public static Vector2 XY(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public static Vector2 XZ(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }
    }
}
