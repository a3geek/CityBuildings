using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NightCity.Creators
{
    using Utilities;
    using Components;
    
    public class RoadPavement
    {
        [Serializable]
        public struct Lane
        {
            public Vector3 start;
            public Vector3 end;
            public float interval;
        }

        [SerializeField]
        private float interval = 1f;
        [SerializeField]
        private float margin = 1f;


        public void Create()
        {
            var sections = CityArea.Instance.Sections;
            var roads = CityArea.Instance.roads;
        }
    }
}
