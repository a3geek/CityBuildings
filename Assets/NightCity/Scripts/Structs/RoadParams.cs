using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Structs
{
    using Structs;
    using Utilities;
    using Random = UnityEngine.Random;

    [Serializable]
    public class MainRoadParams
    {
        public float Width => this.width;
        public float Rate => this.rate;

        [SerializeField]
        private float width = 16f;
        [SerializeField]
        private float rate = 0.1f;
    }

    [Serializable]
    public class SubRoadParams
    {
        public float Width => this.width;

        [SerializeField]
        private float width = 4f;
    }
}