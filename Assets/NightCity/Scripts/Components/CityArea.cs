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
        private float fieldWidth = 1000f;
        [SerializeField]
        private float fieldHeight = 1000f;
        [SerializeField]
        private float sectionWidth = 10f;
        [SerializeField]
        private float sectionHeight = 10f;


        protected override void Awake()
        {
            base.Awake();

            var stepW = this.fieldWidth / this.sectionWidth;
            var stepH = this.fieldHeight / this.sectionHeight;

            var halfStepW = stepW * 0.5f;
            var halfStepH = stepH * 0.5f;

            for(var w = 0f; w < this.fieldWidth; w += stepW)
            {
                for(var h = 0f; h < this.fieldHeight; h += stepH)
                {
                    this.Sections.Add(new Section(
                        new Vector2(w + halfStepW, h + halfStepH),
                        new Vector2(stepW, stepH)
                    ));
                }
            }
        }
    }
}
