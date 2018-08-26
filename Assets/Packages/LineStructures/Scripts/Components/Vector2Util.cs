using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures.Components
{
    public static class Vector2Util
    {
        public static Vector2 Normal(this Vector2 self)
        {
            return new Vector2(-self.y, self.x);
        }
        
        public static float Cross(Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }
    }
}
