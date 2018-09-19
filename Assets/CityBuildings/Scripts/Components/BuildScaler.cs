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
        public const string PropTex = "_Tex";
        public const string PropHeight = "_Height";

        [SerializeField]
        private Camera cam = null;
        [SerializeField]
        private Material material = null;
        [SerializeField]
        private Renderer render = null;
        [SerializeField]
        private Vector2 range = new Vector2(0f, 0.75f);

        private Rect field = new Rect();


        public void Init(WindowTextureManager windowTexture)
        {
            this.cam = this.cam ?? GetComponentInParent<Camera>();
            this.render = this.render ?? GetComponent<MeshRenderer>();

            var center = transform.position.XY();
            var size = transform.localScale.XY();

            this.field = new Rect(center - 0.5f * size, size);

            this.material = new Material(this.material);
            this.material.SetTexture(PropTex, windowTexture.BuildScalerTex);

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
                this.material.SetFloat(PropHeight, 1f);
            }

            if(Input.GetMouseButtonDown(0) == false)
            {
                return;
            }

            var p = Input.mousePosition;
            var pos = this.cam.ScreenToWorldPoint(new Vector3(p.x, p.y, 0f));
            if(this.field.Contains(pos.XY()) == false)
            {
                return;
            }

            var height = pos.y - this.field.yMin;
            var rate = height / this.field.height;

            var range = this.range.x + (this.range.y - this.range.x) * rate;
            Debug.Log(range + " _ " + rate);
            //MainController.Instance.Rebuild(range);
        }
    }
}
