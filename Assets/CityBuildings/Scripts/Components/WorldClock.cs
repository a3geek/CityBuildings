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
    [AddComponentMenu("City Buildings/Components/World Clock")]
    public class WorldClock : MonoBehaviour
    {
        public const string PropBorderColor = "_BorderColor";
        public const string PropZeroColro = "_ZeroColor";
        public const string PropThreeColor = "_ThreeColor";
        public const string PropSixColor = "_SixColor";
        public const string PropNineColor = "_NineColor";
        public const string PropRadius = "_Radius";
        public const string PropBolid = "_Bolid";
        public const string PropRing = "_Ring";
        public const string PropOffset = "_Offset";
        public const string PropBlend = "_Blend";

        [SerializeField]
        private Color border = Color.white;
        [SerializeField]
        private Color zero = new Color(0.003604479f, 0.096407f, 0.254717f, 1f);
        [SerializeField]
        private Color three = new Color(0.8274511f, 0.8862746f, 0.9254903f, 1f);
        [SerializeField]
        private Color six = new Color(0.2235294f, 0.6862745f, 0.8274511f, 1f);
        [SerializeField]
        private Color nine = new Color(0.8784314f, 0.5960785f, 0.3333333f, 1f);
        [SerializeField]
        private Camera cam = null;
        [SerializeField]
        private float radius = 0.5f;
        [SerializeField]
        private float bolid = 0.015f;
        [SerializeField]
        private float ring = 0.075f;
        [SerializeField]
        private float offset = 0.1f;
        [SerializeField]
        private float blend = 0.05f;
        [SerializeField]
        private Material material = null;
        [SerializeField]
        private Renderer render = null;

        private Vector2 center = Vector3.zero;
        private Vector2 range = Vector2.zero;


        private void Awake()
        {
            this.cam = this.cam ?? GetComponentInParent<Camera>();
            this.render = this.render ?? GetComponent<MeshRenderer>();

            this.center = transform.position.XY();
            this.range = new Vector2(
                transform.localScale.x * this.radius,
                transform.localScale.x * this.ring
            );

            this.material = new Material(this.material);
            this.SetProps();

            this.render.material = this.material;
            this.render.enabled = false;
        }

        private void Update()
        {
            if(MainController.Instance.IsPlaying == false)
            {
                return;
            }
            else if(this.render.enabled == false)
            {
                this.render.enabled = true;
            }

            this.SetProps();

            if(Input.GetMouseButtonDown(0) == false)
            {
                return;
            }

            var p = Input.mousePosition;
            var pos = this.cam.ScreenToWorldPoint(new Vector3(p.x, p.y, 0f));
            var diff = pos.XY() - this.center;

            if(diff.sqrMagnitude > this.range.x * this.range.x || diff.sqrMagnitude <= this.range.y * this.range.y)
            {
                return;
            }

            var d = Vector2.Dot(Vector2.up * (diff.y >= 0f ? 1f : -1f), diff) / diff.magnitude - this.offset;
            var select = d <= 0.5f ? (diff.x >= 0f ? 1 : 3) : (diff.y >= 0f ? 0 : 2);

            SkyManager.Instance.SetSky(select);
        }

        private void OnDestroy()
        {
            DestroyImmediate(this.material);
        }

        private void SetProps()
        {
            this.material.SetColor(PropBorderColor, this.border);
            this.material.SetColor(PropZeroColro, this.zero);
            this.material.SetColor(PropThreeColor, this.three);
            this.material.SetColor(PropSixColor, this.six);
            this.material.SetColor(PropNineColor, this.nine);
            this.material.SetFloat(PropRadius, this.radius);
            this.material.SetFloat(PropBolid, this.bolid);
            this.material.SetFloat(PropRing, this.ring);
            this.material.SetFloat(PropOffset, this.offset);
            this.material.SetFloat(PropBlend, this.blend);
        }
    }
}
