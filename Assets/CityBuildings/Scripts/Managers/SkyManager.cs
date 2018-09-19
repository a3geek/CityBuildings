using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

namespace CityBuildings.Managers
{
    using Creators;
    using Structs;
    using Utilities;
    using PostEffects;

    [DisallowMultipleComponent]
    [AddComponentMenu("City Buildings/Managers/Sky Manager")]
    public class SkyManager : SingletonMonoBehaviour<SkyManager>
    {
        public const string PropDofColor = "_DofColor";
        public const string PropDofPower = "_DofPower";

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
            public Color dof = Color.black;
            public float dofPower = 750f;
        }

        public int Current { get; private set; } = 0;
        public bool Validity { get; set; } = false;
        public bool IsFinished { get; private set; } = false;

        [SerializeField]
        private List<SkyboxSettings> settings = new List<SkyboxSettings>();
        [SerializeField]
        private Bloom bloom = null;
        [SerializeField]
        private Material skybox = null;
        [SerializeField]
        private AnimationCurve dofCurve = new AnimationCurve();
        [SerializeField]
        private float dofSpeed = 0.25f;

        private float dof = 0f;

        
        private void Update()
        {
            if(this.Validity == false || this.IsFinished == true)
            {
                return;
            }
            
            this.dof = Mathf.Clamp01(this.dof + Time.deltaTime * this.dofSpeed);
            var clamp = Mathf.Clamp01(this.dofCurve.Evaluate(this.dof));
            var dof = this.settings[this.Current].dofPower * clamp;

            this.IsFinished = clamp >= 1f;
            Shader.SetGlobalFloat(PropDofPower, dof);
        }

        public void Init()
        {
            this.bloom = this.bloom ?? Camera.main.GetComponent<Bloom>();

            this.SetSky(Current);
            Shader.SetGlobalFloat(PropDofPower, 0f);
        }

        public void SetDof(float dof)
        {
            this.dof = dof;
            this.IsFinished = false;
        }
        
        public void SetSky(int index)
        {
            var settings = this.settings[index];

            this.Current = index;
            this.bloom.enabled = index == 0 ? true : false;

            this.skybox.SetColor(PropTopColor, settings.Top);
            this.skybox.SetColor(PropHorizonColor, settings.Horizon);
            this.skybox.SetColor(PropFloorColor, settings.Floor);
            this.skybox.SetFloat(PropHorizon, settings.HorizonLine);
            this.skybox.SetFloat(PropHorizonOffset, settings.HorizonOffset);

            Shader.SetGlobalFloat(PropDofPower, settings.dofPower);
            Shader.SetGlobalColor(PropDofColor, settings.dof);
        }
    }
}
