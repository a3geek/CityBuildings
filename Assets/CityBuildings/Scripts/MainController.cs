using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuildings
{
    using Components;
    using Managers;
    using Utilities;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(WindowTextureManager))]
    [RequireComponent(typeof(SkyscraperManager))]
    [RequireComponent(typeof(RoadsManager))]
    [RequireComponent(typeof(CarsManager))]
    [RequireComponent(typeof(DecorationManager))]
    [RequireComponent(typeof(SkyManager))]
    [AddComponentMenu("City Buildings/Main Controller")]
    public class MainController : SingletonMonoBehaviour<MainController>
    {
        public const string PropIsNight = "_IsNight";

        public bool IsPlaying => this.mover.Validity;
        public bool IsNight => this.Sky.Current == 0;
        public WindowTextureManager WindowTexture { get; private set; }
        public SkyscraperManager SkyScraper { get; private set; }
        public RoadsManager Roads { get; private set; }
        public CarsManager Cars { get; private set; }
        public DecorationManager Decoration { get; private set; }
        public SkyManager Sky { get; private set; }

        [SerializeField]
        private CameraMover mover = null;
        [SerializeField]
        private Load load = null;
        [SerializeField]
        private BuildScaler buildScaler = null;

        private bool ready = false;
        private List<Action> initializes = new List<Action>();


        protected override void Awake()
        {
            base.Awake();

            this.WindowTexture = GetComponent<WindowTextureManager>();
            this.SkyScraper = GetComponent<SkyscraperManager>();
            this.Roads = GetComponent<RoadsManager>();
            this.Cars = GetComponent<CarsManager>();
            this.Decoration = GetComponent<DecorationManager>();
            this.Sky = GetComponent<SkyManager>();

            this.mover = this.mover ?? Camera.main.GetComponent<CameraMover>();
            this.load = this.load ?? Camera.main.GetComponent<Load>();
            this.buildScaler = this.buildScaler ?? FindObjectOfType<BuildScaler>();

            this.initializes = new List<Action>()
            {
                () => this.Sky.Initialize(),
                () => this.WindowTexture.Initialize(0),
                () => this.WindowTexture.Initialize(1),
                () => this.WindowTexture.Initialize(2),
                () => this.WindowTexture.Initialize(3),
                () => this.SkyScraper.Initialize(this.WindowTexture, this.Sky),
                () => this.buildScaler.Initialize(this.SkyScraper),
                () => this.Roads.Initialize(this.SkyScraper),
                () => this.Cars.Initialize(this.SkyScraper),
                () => this.Decoration.Initialize(this.SkyScraper),
                () => this.mover.Initialize(this.SkyScraper)
            };

            StartCoroutine(this.Initialize());
        }

        private void Update()
        {
            if(this.ready == false)
            {
                return;
            }

            if(this.load.Validity == true && this.load.IsFinished == true)
            {
                this.load.Validity = false;
            }
            else if(this.load.Validity == false && this.mover.Validity == false)
            {
                this.Sky.Validity = true;
                this.mover.Validity = this.Cars.Validity = this.Sky.IsFinished;
            }

            Shader.SetGlobalInt(PropIsNight, this.Sky.Current == 0 ? 1 : 0);
        }

        public void Rebuild(float specialRate)
        {
            this.SkyScraper.Build(specialRate);
            this.Decoration.Initialize(this.SkyScraper);
        }

        private IEnumerator Initialize()
        {
            this.load.Initialize();
            yield return new WaitForSeconds(0f);

            var cnt = 0;
            while(cnt < this.initializes.Count)
            {
                this.initializes[cnt++]();

                yield return new WaitForSeconds(0f);
            }

            this.ready = true;
        }
    }
}
