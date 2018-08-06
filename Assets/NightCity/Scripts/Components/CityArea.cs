using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

namespace NightCity.Components
{
    using Utilities;

    [DisallowMultipleComponent]
    [DefaultExecutionOrder(ExecutionOrder)]
    [AddComponentMenu("Night City/Components/City Area")]
    public class CityArea : SingletonMonoBehaviour<CityArea>
    {
        [Serializable]
        public struct Section
        {
            public static float DefaultCenterY = 0f;
            public static float DefaultSizeY = 0f;

            public Vector3 BottomLeft
            {
                get { return this.Center - 0.5f * this.Size; }
            }

            public Vector3 Center { get; }
            public Vector3 Size { get; }


            public Section(Vector2 center, Vector2 size)
            {
                this.Center = new Vector3(center.x, DefaultCenterY, center.y);
                this.Size = new Vector3(size.x, DefaultSizeY, size.y);
            }
        }
        public class Road
        {
            public Vector2 from;
            public Vector2 to;
            public Vector2 normal;
            public float width;


            public Road(Vector2 from, Vector2 to, float width)
            {
                this.from = from;
                this.to = to;
                this.width = width;

                var diff = to - from;
                this.normal = (new Vector2(diff.y, diff.x)).normalized;
            }
            
        }

        public const int ExecutionOrder = -32000;

        public List<Section> Sections { get; } = new List<Section>();
        public List<Road> roads
        {
            get { return this.vertical.Concat(this.horizontal).ToList(); }
        }

        public List<Road> vertical { get; } = new List<Road>();
        public List<Road> horizontal { get; } = new List<Road>();


        [SerializeField]
        private Vector2 field = new Vector2(1000f, 1000f);
        [SerializeField]
        private Vector2 sectionX = new Vector2(15f, 60f);
        [SerializeField]
        private Vector2 sectionY = new Vector2(15f, 60f);
        
        [Header("Road"), Space]
        [SerializeField]
        private float mainRoadWidth = 8f;
        [SerializeField]
        private float subRoadWidth = 4f;
        [SerializeField]
        private float mainRate = 0.1f;
        

        protected override void Awake()
        {
            base.Awake();
            this.Init();
        }

        private List<Road> CreateLanes(Vector2 section, Vector2 dir, Vector4 field)
        {
            var lanes = new List<Road>();
            var main = this.mainRoadWidth;

            var reverse = -1f * (dir - Vector2.one);

            var bypass = true;
            var last = field.y - main * 0.5f;
            var step = field.x + main;
            do
            {
                var sector = Random.Range(section.x, section.y);
                step += sector;

                if(step > last || last - step <= this.mainRoadWidth + section.x)
                {
                    step = last;
                    break;
                }

                bypass = bypass ? false : Random.value < this.mainRate;
                var width = bypass ? this.mainRoadWidth : this.subRoadWidth;

                lanes.Add(new Road(
                    (step + width * 0.5f) * dir + reverse * field.z,
                    (step + width * 0.5f) * dir + reverse * field.w,
                    width
                ));

                step += width;
            }
            while(true);

            return lanes;
        }

        private void CreateLanes(Vector4 section, Vector4 field)
        {
            var main = this.mainRoadWidth;

            this.vertical.AddRange(new List<Road>(){
                new Road(new Vector2(field.x + main * 0.5f, field.z), new Vector2(field.x + main * 0.5f, field.w), main),
                new Road(new Vector2(field.y - main * 0.5f, field.z), new Vector2(field.y - main * 0.5f, field.w), main)
            });
            this.horizontal.AddRange(new List<Road>()
            {
                new Road(new Vector2(field.x, field.z + main * 0.5f), new Vector2(field.y, field.z + main * 0.5f), main),
                new Road(new Vector2(field.x, field.w - main * 0.5f), new Vector2(field.y, field.w - main * 0.5f), main)
            });
            
            this.vertical.AddRange(this.CreateLanes(
                new Vector2(section.x, section.y), new Vector2(1f, 0f), field
            ));

            this.horizontal.AddRange(this.CreateLanes(
                new Vector2(section.z, section.w), new Vector2(0f, 1f), new Vector4(field.z, field.w, field.x, field.y)
            ));
        }

        private void Init()
        {
            this.CreateLanes(
                new Vector4(this.sectionX.x, this.sectionX.y, this.sectionY.x, this.sectionY.y),
                new Vector4(-this.field.x, this.field.x, -this.field.y, this.field.y)
            );


            for(var i = 0; i < this.horizontal.Count; i++)
            {
                var h = this.horizontal[i];

                for(var j = 0; j < this.vertical.Count; j++)
                {
                    var v = this.vertical[j];


                }
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            var roads = this.roads;
            for(var i = 0; i < roads.Count; i++)
            {
                var road = roads[i];

                var diff = road.to - road.from;
                var size = diff + road.normal * road.width;
                var center = road.from + diff * 0.5f;
                
                Gizmos.DrawCube(new Vector3(center.x, 0f, center.y), new Vector3(size.x, 0f, size.y));
            }
        }
    }
}
