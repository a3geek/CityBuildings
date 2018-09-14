using System;
using UnityEngine;

namespace NightCity.Utilities
{
    using Random = UnityEngine.Random;

    public static class Vector2Extends
    {
        public static float Rand(this Vector2 vec2)
        {
            return Random.Range(vec2.x, vec2.y);
        }

        public static float Average(this Vector2 vec2)
        {
            return (vec2.x + vec2.y) * 0.5f;
        }

        public static Vector2 Surplus(this Vector2 v2, float v)
        {
            return new Vector2(v2.x % v, v2.y % v);
        }

        public static Vector3 ToVector3(this Vector2 v2, float y = 0f)
        {
            return new Vector3(v2.x, y, v2.y);
        }

        public static Vector2 Normal(this Vector2 v2)
        {
            return new Vector2(v2.y, -v2.x).normalized;
        }

        public static void EachAction(this Vector2 v2, Action<float> action)
        {
            action(v2.x);
            action(v2.y);
        }

        public static void EachAction(this Vector2 v2, Action<int, float> action)
        {
            action(0, v2[0]);
            action(1, v2[1]);
        }

        public static void EachAction(this Vector2 v2, Func<int, float, bool> condition, Action<int, float> action)
        {
            if(condition(0, v2[0]) == true)
            {
                action(0, v2[0]);
            }
            if(condition(1, v2[1]) == true)
            {
                action(1, v2[1]);
            }
        }

        public static Vector2 EachFunc(this Vector2 v2, Func<float, float> func)
        {
            return new Vector2(func(v2.x), func(v2.y));
        }

        public static Vector2 EachFunc(this Vector2 v2, Func<int, float, float> func)
        {
            return new Vector2(func(0, v2[0]), func(1, v2[1]));
        }

        public static Vector2 EachFunc(this Vector2 v2, Vector2 other, Func<float, float, float> func)
        {
            return new Vector2(func(v2.x, other.x), func(v2.y, other.y));
        }
    }
}
