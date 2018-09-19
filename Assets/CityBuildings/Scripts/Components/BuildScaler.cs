using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CityBuildings.Components
{
    using Managers;
    using Structs;
    using Utilities;
    using Random = UnityEngine.Random;

    [DisallowMultipleComponent]
    [AddComponentMenu("City Buildings/Components/Build Scaler")]
    public class BuildScaler : MonoBehaviour
    {
        public const string PropHeight = "_Height";
        public const string PropTopColor = "_TopColor";
        public const string PropBottomColor = "_BottomColor";

        [SerializeField]
        private Color top = Color.red;
        [SerializeField]
        private Color bottom = Color.blue;
        [SerializeField]
        private Camera cam = null;
        [SerializeField]
        private Material material = null;
        [SerializeField]
        private Renderer render = null;
        [SerializeField]
        private Vector2 range = new Vector2(0f, 0.75f);

        private Rect field = new Rect();
        private bool downing = false;


        public void Init(SkyscraperManager skyscraper)
        {
            this.cam = this.cam ?? GetComponentInParent<Camera>();
            this.render = this.render ?? GetComponent<MeshRenderer>();

            var center = transform.position.XY();
            var size = transform.localScale.XY();

            this.field = new Rect(center - 0.5f * size, size);

            this.material = new Material(this.material);
            this.material.SetFloat(PropHeight,
                skyscraper.Builder.SpecialRate / (this.range.y - this.range.x)
            );
            this.material.SetColor(PropTopColor, this.top);
            this.material.SetColor(PropBottomColor, this.bottom);

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

            var down = Input.GetMouseButtonDown(0);
            var repeat = Input.GetMouseButton(0);
            var up = Input.GetMouseButtonUp(0);

            if(down == false && repeat == false && up == false)
            {
                this.downing = false;
                return;
            }

            var p = Input.mousePosition;
            var pos = this.cam.ScreenToWorldPoint(new Vector3(p.x, p.y, 0f));
            if(this.field.Contains(pos.XY()) == false && this.downing == false)
            {
                return;
            }

            this.downing = down == true ? true :
                (this.downing == true && (repeat == true || up == true) ? true : false);

            var height = Mathf.Clamp(pos.y - this.field.yMin, 0f, this.field.height);
            var rate = height / this.field.height;

            var range = this.range.x + (this.range.y - this.range.x) * rate;
            this.material.SetFloat(PropHeight, rate);

            if(up == true)
            {
                MainController.Instance.Rebuild(range);
            }
        }
    }
}
