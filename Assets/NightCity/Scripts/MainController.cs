using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity
{
    using Components;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(WindowTexture))]
    [RequireComponent(typeof(Skyscraper))]
    [RequireComponent(typeof(Roadloader))]
    [RequireComponent(typeof(CarsRender))]
    [AddComponentMenu("Night City/Main Controller")]
    public class MainController : MonoBehaviour
    {
        [SerializeField]
        private CameraMover mover = null;
        
        private WindowTexture windowTexture = null;
        private Skyscraper skyScraper = null;
        private Roadloader roadloader = null;
        private CarsRender carsRender = null;


        private void Awake()
        {
            this.windowTexture = GetComponent<WindowTexture>();
            this.windowTexture.Init();

            this.skyScraper = GetComponent<Skyscraper>();
            this.skyScraper.Init(this.windowTexture);

            this.roadloader = GetComponent<Roadloader>();
            this.roadloader.Init(this.skyScraper);

            this.carsRender = GetComponent<CarsRender>();
            this.carsRender.Init(this.skyScraper);

            this.mover = this.mover ?? Camera.main.GetComponent<CameraMover>();
            this.mover.Init(this.skyScraper);
        }
    }
}
