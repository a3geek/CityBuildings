using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

namespace CityBuildings.Components
{
    using Managers;
    using Utilities;
    using Structs;
    using Random = UnityEngine.Random;
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("City Buildings/Components/Camera Mover")]
    public class CameraMover : MonoBehaviour
    {
        public bool Validity { get; set; } = false;

        private Road road => this.skyscraper.CityArea.Roads[this.roadID];
        private Vector2 from => this.isForward == true ? this.road.From : this.road.To;
        private Vector2 to => this.isForward == true ? this.road.To : this.road.From;
        private Vector2 dir => this.isForward == true ? this.road.Direction : -1f * this.road.Direction;
        private bool IsWaiting => this.timer < this.autotime;

        [SerializeField]
        private float speed = 1f;
        [SerializeField]
        private float rotateSpeed = 1f;
        [SerializeField]
        private float fovSpeed = 1f;
        [SerializeField]
        private Vector2 fovRange = new Vector2(30f, 90f);
        [SerializeField, Range(0f, 1f)]
        private float straightRate = 0.75f;
        [SerializeField]
        private float autotime = 5f;

        private int roadID = 0;
        private float progress = 0f;
        private bool isForward = true;
        private float timer = 0f;
        private bool pause = false;
        private float rotateRate = 0f;
        private float fov = 0f;
        private SkyscraperManager skyscraper = null;


        public void Init(SkyscraperManager skyscraper)
        {
            this.skyscraper = skyscraper;

            var ids = skyscraper.CityArea.Roads.Keys;
            this.roadID = ids.ElementAt(Random.Range(0, ids.Count));
            this.progress = Random.Range(0f, this.road.Magnitude);

            this.rotateRate = 1f;
            this.timer = this.autotime;
            this.fov = Camera.main.fieldOfView;

            transform.rotation = this.Auto();
        }

        private void Update()
        {
            if(this.skyscraper == null || this.Validity == false)
            {
                return;
            }

            this.Manual();
            this.Auto();
        }

        private bool Manual()
        {
            var right = Input.GetKey(KeyCode.RightArrow);
            var left = Input.GetKey(KeyCode.LeftArrow);
            var up = Input.GetKey(KeyCode.UpArrow);
            var down = Input.GetKey(KeyCode.DownArrow);
            var space = Input.GetKeyDown(KeyCode.Space);

            if(right == false && left == false && space == false && up == false && down == false)
            {
                return false;
            }

            var euler = transform.rotation.eulerAngles;
            euler.y += (this.speed * Time.deltaTime * ((right == true ? 1f : 0f) + (left == true ? -1f : 0f)));
            transform.rotation = Quaternion.Euler(euler);

            Camera.main.fieldOfView = Mathf.Clamp(
                Camera.main.fieldOfView + this.fovSpeed * Time.deltaTime * (up == true ? -1f : (down == true ? 1f : 0f)),
                this.fovRange.x, this.fovRange.y
            );

            this.pause = space == true ? !this.pause : this.pause;
            this.timer = 0f;
            this.rotateRate = 0f;

            return true;
        }
        
        private Quaternion Auto()
        {
            this.timer += Time.deltaTime * (this.IsWaiting == true && this.pause == false ? 1f : 0f);
            this.AutoMove();

            if(this.IsWaiting == false)
            {
                this.rotateRate = Mathf.Clamp01(this.rotateRate + Time.deltaTime * this.rotateSpeed);
            }

            var pos = transform.position;
            var field = this.skyscraper.CityArea.Field;
            var center = this.skyscraper.CityArea.FieldCenter;

            var dis = pos.XZ() - center;
            var rot = Quaternion.LookRotation(center.ToVector3(pos.y) - pos);
            var rate = Mathf.Clamp01(Mathf.Max(Mathf.Abs(dis.x / field.x), Mathf.Abs(dis.y / field.y))) * this.rotateRate;

            var fov = Camera.main.fieldOfView;
            Camera.main.fieldOfView = fov + (this.fov - fov) * this.rotateRate;

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rate);

            return rot;
        }

        private void AutoMove()
        {
            var pos = transform.position;

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
                this.progress = Mathf.Min(
                    this.road.Magnitude,
                    this.progress + this.speed * Time.deltaTime * (this.pause == true ? 0f : 1f)
                );
            }

            transform.position = this.pause == true ? pos :
                Vector2.Lerp(this.from, this.to, this.GetProgress()).ToVector3(pos.y);
        }

        private float GetProgress()
        {
            return this.progress / this.road.Magnitude;
        }
    }
}
