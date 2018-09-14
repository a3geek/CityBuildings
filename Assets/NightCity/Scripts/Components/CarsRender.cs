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
        public struct Data
        {
            public Vector2 pos;
            public Vector2 dir;
        }

        public const string PropHeight = "_Height";
        public const string PropSize = "_Size";
        public const string PropForwardColor = "_ForwardColor";
        public const string PropBackColor = "_BackColor";
        public const string PropGeomData = "_GeomData";

        [SerializeField]
        private int num = 50;
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

            this.geomBuffer = new ComputeBuffer(this.cars.Count, Marshal.SizeOf(typeof(Data)), ComputeBufferType.Default);
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

            var data = this.cars.ConvertAll(car =>
            {
                var road = this.skyscraper.CityArea.Roads[car.RoadID];
                var dir = car.Dir == 1 ? road.Direction : -1f * road.Direction;
                var normal = (new Vector2(dir.y, -dir.x)).normalized * this.offset;

                return new Data()
                {
                    pos = car.GetPos(road) + normal,
                    dir = dir
                };
            });

            this.geomBuffer.SetData(data.ToArray());
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
