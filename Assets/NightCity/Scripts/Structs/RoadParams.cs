﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Structs
{
    [Serializable]
    public class MainRoadParams
    {
        public float Width => this.width;
        public float Rate => this.rate;

        [SerializeField]
        private float width = 24f;
        [SerializeField]
        private float rate = 0.2f;
    }

    [Serializable]
    public class SubRoadParams
    {
        public float Width => this.width;

        [SerializeField]
        private float width = 12f;
    }
}