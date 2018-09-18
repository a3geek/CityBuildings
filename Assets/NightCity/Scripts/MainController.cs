using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity
{
    using Components;
    using Managers;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(WindowTextureManager))]
    [RequireComponent(typeof(SkyscraperManager))]
    [RequireComponent(typeof(RoadsManager))]
    [RequireComponent(typeof(CarsManager))]
    [RequireComponent(typeof(DecorationManager))]
    [AddComponentMenu("Night City/Main Controller")]
    public class MainController : MonoBehaviour
    {
        public const string PropDofPower = "_DofPower";

        [SerializeField]
        private CameraMover mover = null;
        [SerializeField]
        private Load load = null;
        [SerializeField]
        private float dofPower = 750f;

        private bool inited = false;
        private WindowTextureManager windowTexture = null;
        private SkyscraperManager skyScraper = null;
        private RoadsManager roads = null;
        private CarsManager cars = null;
        private DecorationManager decoration = null;


        private void Awake()
        {
            Shader.SetGlobalFloat(PropDofPower, this.dofPower);

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
            if(this.inited == true && this.load.Validity == true && this.load.IsFinished == true)
            {
                this.load.Validity = false;
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
