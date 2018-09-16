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
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Night City/Components/Camera Mover")]
    public class CameraMover : MonoBehaviour
    {
        private Road road => this.skyscraper.CityArea.Roads[this.roadID];
        private Vector2 from => this.isForward == true ? this.road.From : this.road.To;
        private Vector2 to => this.isForward == true ? this.road.To : this.road.From;
        private Vector2 dir => this.isForward == true ? this.road.Direction : -1f * this.road.Direction;

        [SerializeField]
        private float speed = 1f;
        [SerializeField, Range(0f, 1f)]
        private float straightRate = 0.75f;
        [SerializeField]
        private float autotime = 5f;

        private int roadID = 0;
        private float progress = 0f;
        private bool isForward = true;
        private float timer = 0f;
        private Skyscraper skyscraper = null;


        public void Init(Skyscraper skyscraper)
        {
            this.skyscraper = skyscraper;

            var ids = skyscraper.CityArea.Roads.Keys;
            this.roadID = ids.ElementAt(Random.Range(0, ids.Count));
            this.progress = Random.Range(0f, this.road.Magnitude);
        }

        private void Update()
        {
            var pos = transform.position;
            var field = this.skyscraper.CityArea.Field;
            var center = this.skyscraper.CityArea.FieldCenter;

            if(this.GetProgress() >= 1f)
            {
                var to = this.to;

                this.roadID = RoadPointer.GetNextRoadID(
                    this.road,
                    this.isForward == true ? this.road.ToPointID : this.road.FromPointID,
                    this.skyscraper.CityArea, this.straightRate
                );

                this.isForward = this.road.IsForward(to);
                this.progress = 0f;
            }
            else
            {
                this.progress = Mathf.Min(this.road.Magnitude, this.progress + this.speed);
            }

            var dis = pos.XZ() - center;
            var rate = Mathf.Clamp01(Mathf.Max(Mathf.Abs(dis.x / field.x), Mathf.Abs(dis.y / field.y)));
            var rot = Quaternion.LookRotation(center.ToVector3(pos.y) - pos);

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rate);
            transform.position = Vector2.Lerp(this.from, this.to, this.GetProgress()).ToVector3(pos.y);
        }

        private float GetProgress()
        {
            return this.progress / this.road.Magnitude;
        }
    }
}
