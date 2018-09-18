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
        [SerializeField]
        private CameraMover mover = null;
        
        private WindowTextureManager windowTexture = null;
        private SkyscraperManager skyScraper = null;
        private RoadsManager roads = null;
        private CarsManager cars = null;
        private DecorationManager decoration = null;


        private void Awake()
        {
            this.windowTexture = GetComponent<WindowTextureManager>();
            this.windowTexture.Init();

            this.skyScraper = GetComponent<SkyscraperManager>();
            this.skyScraper.Init(this.windowTexture);

            this.roads = GetComponent<RoadsManager>();
            this.roads.Init(this.skyScraper);

            this.cars = GetComponent<CarsManager>();
            this.cars.Init(this.skyScraper);

            this.decoration = GetComponent<DecorationManager>();
            this.decoration.Init(this.skyScraper);

            this.mover = this.mover ?? Camera.main.GetComponent<CameraMover>();
            this.mover.Init(this.skyScraper);
        }
    }
}
