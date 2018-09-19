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
        public bool IsNight => this.sky.Current == 0;

        [SerializeField]
        private CameraMover mover = null;
        [SerializeField]
        private Load load = null;
        [SerializeField]
        private BuildScaler buildScaler = null;

        private bool ready = false;
        private WindowTextureManager windowTexture = null;
        private SkyscraperManager skyScraper = null;
        private RoadsManager roads = null;
        private CarsManager cars = null;
        private DecorationManager decoration = null;
        private SkyManager sky = null;


        protected override void Awake()
        {
            base.Awake();

            this.windowTexture = GetComponent<WindowTextureManager>();
            this.skyScraper = GetComponent<SkyscraperManager>();
            this.roads = GetComponent<RoadsManager>();
            this.cars = GetComponent<CarsManager>();
            this.decoration = GetComponent<DecorationManager>();
            this.mover = this.mover ?? Camera.main.GetComponent<CameraMover>();
            this.sky = this.sky ?? SkyManager.Instance;

            this.buildScaler = this.buildScaler ?? FindObjectOfType<BuildScaler>();

            this.load = this.load ?? Camera.main.GetComponent<Load>();
            this.load.Init();

            StartCoroutine(this.Init());
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
                this.sky.Validity = true;
                this.mover.Validity = this.cars.Validity = this.sky.IsFinished;
            }

            Shader.SetGlobalInt(PropIsNight, SkyManager.Instance.Current == 0 ? 1 : 0);
        }

        public void Rebuild(float specialRate)
        {
            this.skyScraper.Build(specialRate);
            this.decoration.Init(this.skyScraper);
        }

        private IEnumerator Init()
        {
            yield return new WaitForSeconds(0f);

            var cnt = 0;
            while(cnt < 12)
            {
                switch(cnt)
                {
                    case 0:
                        this.sky.Init();
                        break;
                    case 1:
                        this.windowTexture.Init(0);
                        break;
                    case 2:
                        this.windowTexture.Init(1);
                        break;
                    case 3:
                        this.windowTexture.Init(2);
                        break;
                    case 4:
                        this.windowTexture.Init(3);
                        break;
                    case 5:
                        this.skyScraper.Init(this.windowTexture, this.sky);
                        break;
                    case 6:
                        this.buildScaler.Init(this.skyScraper);
                        break;
                    case 7:
                        this.roads.Init(this.skyScraper);
                        break;
                    case 8:
                        this.cars.Init(this.skyScraper);
                        break;
                    case 9:
                        this.decoration.Init(this.skyScraper);
                        break;
                    case 10:
                        this.mover.Init(this.skyScraper);
                        break;
                }
                
                cnt++;
                yield return new WaitForSeconds(0f);
            }

            this.ready = true;
        }
    }
}
