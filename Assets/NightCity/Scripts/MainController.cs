using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity
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
    [AddComponentMenu("Night City/Main Controller")]
    public class MainController : SingletonMonoBehaviour<MainController>
    {
        public const string PropDofPower = "_DofPower";

        public bool IsPlaying => this.mover.Validity;

        [SerializeField]
        private CameraMover mover = null;
        [SerializeField]
        private Load load = null;
        [SerializeField]
        private float dofPower = 750f;
        [SerializeField]
        private AnimationCurve dofCurve = new AnimationCurve();
        [SerializeField]
        private float dofSpeed = 0.1f;

        private bool inited = false;
        private float dof = 0f;
        private WindowTextureManager windowTexture = null;
        private SkyscraperManager skyScraper = null;
        private RoadsManager roads = null;
        private CarsManager cars = null;
        private DecorationManager decoration = null;


        protected override void Awake()
        {
            base.Awake();
            Shader.SetGlobalFloat(PropDofPower, this.dof);

            this.windowTexture = GetComponent<WindowTextureManager>();
            this.skyScraper = GetComponent<SkyscraperManager>();
            this.roads = GetComponent<RoadsManager>();
            this.cars = GetComponent<CarsManager>();
            this.decoration = GetComponent<DecorationManager>();
            this.mover = this.mover ?? Camera.main.GetComponent<CameraMover>();

            this.load = this.load ?? Camera.main.GetComponent<Load>();
            this.load.Init();

            StartCoroutine(this.Init());
        }

        private void Update()
        {
            if(this.inited == false)
            {
                return;
            }

            if(this.load.Validity == true && this.load.IsFinished == true)
            {
                this.load.Validity = false;
            }
            else if(this.load.Validity == false && this.mover.Validity == false)
            {
                this.dof = Mathf.Clamp01(this.dof + Time.deltaTime * this.dofSpeed);
                var dof = this.dofPower * Mathf.Clamp01(this.dofCurve.Evaluate(this.dof));
                Shader.SetGlobalFloat(PropDofPower, dof);

                this.mover.Validity = this.cars.Validity = this.dof >= 1f;
            }
        }

        private IEnumerator Init()
        {
            yield return new WaitForSeconds(0f);

            var cnt = 0;
            while(cnt < 6)
            {
                switch(cnt)
                {
                    case 0:
                        this.windowTexture.Init();
                        break;
                    case 1:
                        this.skyScraper.Init(this.windowTexture);
                        break;
                    case 2:
                        this.roads.Init(this.skyScraper);
                        break;
                    case 3:
                        this.cars.Init(this.skyScraper);
                        break;
                    case 4:
                        this.decoration.Init(this.skyScraper);
                        break;
                    case 5:
                        this.mover.Init(this.skyScraper);
                        break;
                }
                
                cnt++;
                yield return new WaitForSeconds(0f);
            }

            this.inited = true;
        }
    }
}
