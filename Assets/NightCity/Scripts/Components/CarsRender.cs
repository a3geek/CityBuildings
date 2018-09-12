using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NightCity.Components
{
    using Creators;
    using Structs;

    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Cars Render")]
    public class CarsRender : MonoBehaviour
    {
        public class Car
        {
            public int CarIndex = 0;
            public float Value = 0f;
        }

        [SerializeField]
        private int num = 50;

        private List<Car> cars = new List<Car>();
        private Skyscraper skyscraper;

        public void Init(Skyscraper skyscraper)
        {
            this.skyscraper = skyscraper;
            var roads = skyscraper.CityArea.Roads;

            for(var i = 0; i < this.num; i++)
            {
                this.cars.Add(new Car()
                {
                    CarIndex = UnityEngine.Random.Range(0, roads.Count),
                    Value = UnityEngine.Random.value
                });
            }
        }

        private void OnDrawGizmos()
        {
            if(this.skyscraper == null)
            {
                return;
            }

            var roads = this.skyscraper.CityArea.Roads;

            for(var i = 0; i < this.cars.Count; i++)
            {
                var car = this.cars[i];
                var road = roads[car.CarIndex];

                var distance = (road.To - road.From).magnitude;
                var dir = (road.To - road.From).normalized;

                var pos = road.From + dir * distance * car.Value;
                Gizmos.DrawSphere(new Vector3(pos.x, 0f, pos.y), 10f);
            }
        }
    }
}
