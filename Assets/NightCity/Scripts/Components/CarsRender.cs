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

    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Cars Render")]
    public class CarsRender : MonoBehaviour
    {
        [SerializeField]
        private int num = 50;
        [SerializeField]
        private float speed = 1f;

        private List<Car> cars = new List<Car>();
        private Skyscraper skyscraper;

        public void Init(Skyscraper skyscraper)
        {
            this.skyscraper = skyscraper;

            var roads = skyscraper.CityArea.Roads;
            var ids = roads.Keys.ToList();

            for(var i = 0; i < this.num; i++)
            {
                this.cars.Add(new Car()
                {
                    RoadID = ids[UnityEngine.Random.Range(0, ids.Count)],
                    Progress = UnityEngine.Random.value,
                    Dir = UnityEngine.Random.value < 0.5f ? 1 : -1
                });
            }
        }

        private void Update()
        {
            for(var i = 0; i < this.cars.Count; i++)
            {
                var car = this.cars[i];
                var road = this.skyscraper.CityArea.Roads[car.RoadID];
                
                if(car.GetProgress(road) >= 1f)
                {
                    var roadsID = RoadPointer.GetRoadsID(car.Dir == 1 ? road.ToPointID : road.FromPointID)
                        .Where(id => id != road.Id).ToList();
                    
                    var nextID = roadsID.Rand();
                    var next = this.skyscraper.CityArea.Roads[nextID];

                    car.RoadID = nextID;
                    car.Progress = 0f;
                    car.Dir = ((car.Dir == 1 ? road.To : road.From) == next.From ? 1 : -1);
                }
                else
                {
                    car.Progress += this.speed;
                }

                this.cars[i] = car;
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
                var road = roads[car.RoadID];

                var pos = car.GetPos(road);

                Gizmos.color = car.Dir == 1 ? Color.white : Color.red;
                Gizmos.DrawSphere(new Vector3(pos.x, 0f, pos.y), 10f);
            }
        }
    }
}
