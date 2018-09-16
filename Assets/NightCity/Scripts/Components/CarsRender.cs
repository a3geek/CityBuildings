using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

namespace NightCity.Components
{
    using Creators;
    using Utilities;
    using Structs;

    using Random = UnityEngine.Random;

    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Cars Render")]
    public class CarsRender : MonoBehaviour
    {
        public const string PropHeight = "_Height";
        public const string PropSize = "_Size";
        public const string PropForwardColor = "_ForwardColor";
        public const string PropBackColor = "_BackColor";
        public const string PropGeomData = "_GeomData";

        [SerializeField]
        private int num = 50;
        [SerializeField, Range(0f, 1f)]
        private float straightRate = 0.75f;
        [SerializeField]
        private float speed = 1f;
        [SerializeField]
        private float offset = 1f;
        [SerializeField]
        private float size = 1f;
        [SerializeField]
        private float height = 0f;
        [SerializeField]
        private Color forward = Color.white;
        [SerializeField]
        private Color back = Color.red;
        [SerializeField]
        private Material material = null;

        private ComputeBuffer geomBuffer = null;

        private Car[] cars = new Car[0];
        private SimpleCar[] simpleCars = new SimpleCar[0];
        private Skyscraper skyscraper = null;


        public void Init(Skyscraper skyscraper)
        {
            this.skyscraper = skyscraper;

            var roads = skyscraper.CityArea.Roads;
            var ids = roads.Keys;

            this.cars = new Car[this.num];
            this.simpleCars = new SimpleCar[this.num];

            for(var i = 0; i < this.num; i++)
            {
                var id = ids.ElementAt(Random.Range(0, ids.Count));
                this.cars[i] = new Car(roads[id], this.offset);
            }

            this.geomBuffer = new ComputeBuffer(this.cars.Length, Marshal.SizeOf(typeof(SimpleCar)), ComputeBufferType.Default);
        }

        private void Update()
        {
            for(var i = 0; i < this.cars.Length; i++)
            {
                var car = this.cars[i];
                car.Update(this.skyscraper.CityArea, this.speed, this.straightRate);

                this.cars[i] = car;
                this.simpleCars[i] = car;
            }
            
            this.geomBuffer.SetData(this.simpleCars.ToArray());
        }

        private void OnRenderObject()
        {
            if(this.geomBuffer.count <= 0)
            {
                return;
            }

            this.material.SetPass(0);

            this.material.SetFloat(PropHeight, this.height);
            this.material.SetFloat(PropSize, this.size);
            this.material.SetColor(PropForwardColor, this.forward);
            this.material.SetColor(PropBackColor, this.back);
            this.material.SetBuffer(PropGeomData, this.geomBuffer);

            Graphics.DrawProcedural(MeshTopology.Points, 1, this.geomBuffer.count);
        }

        private void OnDestroy()
        {
            this.geomBuffer?.Release();
        }
    }
}
