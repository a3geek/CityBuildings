using UnityEngine;

namespace CityBuildings.Utilities
{
    public static class Vector4Extends
    {
        public static Vector2 XY(this Vector4 v4)
        {
            return new Vector2(v4.x, v4.y);
        }

        public static Vector2 ZW(this Vector4 v4)
        {
            return new Vector2(v4.z, v4.w);
        }
    }
}
