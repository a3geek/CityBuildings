using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Utilities
{
    using Random = UnityEngine.Random;

    public static class ListExtends
    {
        public static T Rand<T>(this List<T> list)
        {
            return list.Count <= 0 ? default(T) : list[Random.Range(0, list.Count)];
        }
    }
}
