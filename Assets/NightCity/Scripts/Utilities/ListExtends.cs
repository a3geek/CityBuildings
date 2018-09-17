using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool IsContains<T>(this List<T> list, Func<T, bool> func)
        {
            for(var i = 0; i < list.Count; i++)
            {
                if(func(list[i]) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            return list.OrderBy(e => Guid.NewGuid()).ToList();
        }
    }
}
