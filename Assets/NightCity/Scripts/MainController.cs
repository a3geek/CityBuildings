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
    [AddComponentMenu("Night City/Main Controller")]
    public class MainController : MonoBehaviour
    {
        [SerializeField]
        private CameraMover mover = null;
        
        private WindowTextureManager windowTexture = null;
        private SkyscraperManager skyScraper = null;
        private RoadsManager roadloader = null;
        private CarsManager carsRender = null;


        private void Awake()
        {
            this.windowTexture = GetComponent<WindowTextureManager>();
            this.windowTexture.Init();

            this.skyScraper = GetComponent<SkyscraperManager>();
            this.skyScraper.Init(this.windowTexture);

            this.roadloader = GetComponent<RoadsManager>();
            this.roadloader.Init(this.skyScraper);

            this.carsRender = GetComponent<CarsManager>();
            this.carsRender.Init(this.skyScraper);

            this.mover = this.mover ?? Camera.main.GetComponent<CameraMover>();
            this.mover.Init(this.skyScraper);
        }
    }
}
