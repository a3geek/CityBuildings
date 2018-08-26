using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures.Examples.Components
{
    using LineStructures.Components;

    [ExecuteInEditMode]
    public abstract class AbstractLineChecker : MonoBehaviour
    {
        public abstract ILine Line
        {
            get;
        }

        [Header("Common Params")]
        public bool Execute = true;
        public bool IsCrossing = false;
        public Color GizmosColor = Color.red;

        [SerializeField]
        protected GameObject p0;
        [SerializeField]
        protected GameObject p1;


        protected void Update()
        {
            if(this.Execute == false)
            {
                return;
            }

            this.CheckObjects();
            this.UpdateLine();
        }

        protected void OnDrawGizmos()
        {
            if(this.Execute == false)
            {
                return;
            }
            var c = Gizmos.color;

            var color = this.GizmosColor;
            color.a = this.IsCrossing == true ? 1f : 0.5f;
            Gizmos.color = color;
            
            this.DrawGizmos();

            this.IsCrossing = false;
            Gizmos.color = c;
        }

        protected void CheckObjects()
        {
#pragma warning disable CS1692
#pragma warning disable IDE0029
            this.p0 = this.p0 == null ? this.CreateSphere("P0", Vector3.zero) : this.p0;
            this.p1 = this.p1 == null ? this.CreateSphere("P1", new Vector3(10f, 0f, 10f)) : this.p1;
#pragma warning disable CS1692
#pragma warning restore IDE0029
        }

        protected GameObject CreateSphere(string name, Vector3 pos)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = name;
            obj.transform.parent = transform;
            obj.transform.position = pos;

            var coll = obj.GetComponent<SphereCollider>();
            if(coll != null)
            {
                DestroyImmediate(coll);
            }

            var render = obj.GetComponent<MeshRenderer>();
            if(render != null)
            {
                render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                render.receiveShadows = false;
            }

            return obj;
        }

        protected Vector2 ToV2FromV3(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        protected abstract void DrawGizmos();
        protected abstract void UpdateLine();
    }
}
