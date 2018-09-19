using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

namespace NightCity.Managers
{
    using Creators;
    using Structs;
    using Utilities;
    using PostEffects;

    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Managers/Skybox Manager")]
    public class SkyboxManager : SingletonMonoBehaviour<SkyboxManager>
    {
        public const string PropTopColor = "_TopColor";
        public const string PropHorizonColor = "_HorizonColor";
        public const string PropFloorColor = "_FloorColor";
        public const string PropHorizon = "_Horizon";
        public const string PropHorizonOffset = "_HorizonOffset";

        [Serializable]
        private class SkyboxSettings
        {
            public Color Top = Color.black;
            public Color Horizon = Color.black;
            public Color Floor = Color.black;
            public float HorizonLine = -0.03f;
            public float HorizonOffset = 0.85f;
        }

        [SerializeField]
        private List<SkyboxSettings> settings = new List<SkyboxSettings>();
        [SerializeField]
        private Bloom bloom = null;
        [SerializeField]
        private Material skybox = null;


        protected override void Awake()
        {
            base.Awake();

            this.SetSky(0);
            this.bloom = this.bloom ?? Camera.main.GetComponent<Bloom>();
        }

        public void SetSky(int index)
        {
            var settings = this.settings[index];

            this.bloom.enabled = index == 0 ? true : false;
            this.skybox.SetColor(PropTopColor, settings.Top);
            this.skybox.SetColor(PropHorizonColor, settings.Horizon);
            this.skybox.SetColor(PropFloorColor, settings.Floor);
            this.skybox.SetFloat(PropHorizon, settings.HorizonLine);
            this.skybox.SetFloat(PropHorizonOffset, settings.HorizonOffset);
        }
    }
}
