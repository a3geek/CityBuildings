using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
            public Vector3 Center { get; }
            public Vector3 Size { get; }


            public Section(Vector2 center, Vector2 size)
            {
                this.Center = new Vector3(center.x, 0f, center.y);
                this.Size = new Vector3(size.x, 0f, size.y);
            }
        }

        public const int ExecutionOrder = -3000;

        public List<Section> Sections { get; } = new List<Section>();

        [SerializeField]
        private Vector2 field = new Vector2(1000f, 1000f);
        [SerializeField]
        private Vector2 section = new Vector2(10f, 10f);
        [SerializeField]
        private float roadWidth = 5f;
        

        protected override void Awake()
        {
            base.Awake();
            
            this.Init();
        }

        private void Init()
        {
            var road = Vector2.one * this.roadWidth;
            var max = this.field - road;

            var region = this.section + road;
            var half = 0.5f * this.section;

            var stepup = max / region;
            var step = new Vector2Int(Mathf.CeilToInt(stepup.x), Mathf.CeilToInt(stepup.y));

            for(var w = 0; w < step.x; w++)
            {
                for(var h = 0; h < step.y; h++)
                {
                    var vec = new Vector2(w, h);

                    this.Sections.Add(new Section(
                        vec * (road + half),
                        half
                    ));
                }
            }
        }

        private void OnDrawGizmos()
        {
            foreach(var e in this.Sections)
            {
                Gizmos.DrawCube(e.Center, e.Size);
            }
        }
    }
}
