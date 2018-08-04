﻿using System.Collections;
using System.Collections.Generic;
using System;
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
        public List<Road> roads { get; } = new List<Road>();

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

        //private List<Road> CreateLanes(float min, float max, float left, float right, float top, float bottom)
        //private List<Road> CreateLanes(Vector2 range, Vector4 field, Vector2 dir, Vector4 line)
        private List<Road> CreateLanes(Vector4 section, Vector4 field)
        {
            var main = this.mainRoadWidth;

            var lanes = new List<Road>()
            {
                new Road(new Vector2(field.x + main * 0.5f, field.z), new Vector2(field.x + main * 0.5f, field.w), main),
                new Road(new Vector2(field.y - main * 0.5f, field.z), new Vector2(field.y - main * 0.5f, field.w), main),

                new Road(new Vector2(field.x, field.z + main * 0.5f), new Vector2(field.y, field.z + main * 0.5f), main),
                new Road(new Vector2(field.x, field.w - main * 0.5f), new Vector2(field.y, field.w - main * 0.5f), main)
            };

            var bypass = false;
            var right = field.y - main * 0.5f;
            var step = field.x + main;
            do
            {
                var sector = Random.Range(section.x, section.y);
                step += sector;

                if(step > right || right - step <= this.mainRoadWidth + section.x)
                {
                    step = right;
                    break;
                }

                bypass = bypass ? false : Random.value < this.mainRate;
                var width = bypass ? this.mainRoadWidth : this.subRoadWidth;

                lanes.Add(new Road(
                    new Vector2(step + width * 0.5f, field.z),
                    new Vector2(step + width * 0.5f, field.w),
                    width
                ));

                step += width;
            }
            while(true);

            var bottom = field.w - main * 0.5f;
            step = field.z + main;
            do
            {
                var sector = Random.Range(section.z, section.w);
                step += sector;

                if(step > bottom || bottom - step <= this.mainRoadWidth + section.z)
                {
                    step = bottom;
                    break;
                }

                bypass = bypass ? false : Random.value < this.mainRate;
                var width = bypass ? this.mainRoadWidth : this.subRoadWidth;

                lanes.Add(new Road(
                    new Vector2(field.x, step + width * 0.5f),
                    new Vector2(field.y, step + width * 0.5f),
                    width
                ));

                step += width;
            }
            while(true);

            return lanes;
        }

        private void Init()
        {
            this.roads.AddRange(this.CreateLanes(
                new Vector4(this.sectionX.x, this.sectionX.y, this.sectionY.x, this.sectionY.y),
                new Vector4(-this.field.x, this.field.x, -this.field.y, this.field.y)
            ));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for(var i = 0; i < this.roads.Count; i++)
            {
                var road = this.roads[i];

                var diff = road.to - road.from;
                var size = diff + road.normal * road.width;
                var center = road.from + diff * 0.5f;
                
                Gizmos.DrawCube(new Vector3(center.x, 0f, center.y), new Vector3(size.x, 0f, size.y));
            }
        }
    }
}
