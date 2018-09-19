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
    [AddComponentMenu("City Buildings/Components/Load")]
    public class Load : MonoBehaviour
    {
        public const string PropTimer = "_Timer";
        public const string PropColor = "_Color";
        public const string PropRadius = "_Radius";
        public const string PropBolid = "_Bolid";

        public bool Validity { get; set; } = true;
        public bool IsFinished => this.timer >= 1f;

        [SerializeField]
        private Material material;
        [SerializeField]
        private float speed = 0.5f;
        [SerializeField]
        private Color color = new Color(1f, 1f, 0f, 1f);
        [SerializeField, Range(0f, 1f)]
        private float radius = 0.2f;
        [SerializeField, Range(0.0001f, 0.5f)]
        private float bolid = 0.01f;

        private float timer = 0f;


        public void Init()
        {
            this.material.SetFloat(PropTimer, 0f);
            this.material.SetColor(PropColor, this.color);
            this.material.SetFloat(PropRadius, this.radius);
            this.material.SetFloat(PropBolid, this.bolid);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(this.material == null || this.Validity == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            this.timer += Time.deltaTime * this.speed;

            this.material.SetFloat(PropTimer, this.timer);
            Graphics.Blit(source, destination, this.material);
        }
    }
}
