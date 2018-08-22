using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostEffects
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class Bloom : MonoBehaviour
    {
        public const string PropDelta = "_Delta";
        public const string PropFilters = "_Filters";
        public const string PropIntensity = "_Intensity";
        public const string PropSourceTex = "_SourceTex";
        public const int MaxIteration = 10;

        [SerializeField]
        private bool validity = true;
        [SerializeField]
        private Material material = null;
        [SerializeField, Range(1, MaxIteration)]
        private int iteration = 3;
        [SerializeField, Range(0f, 1f)]
        private float threshold = 0.1f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float softThreshold = 0.1f;
        [SerializeField, Range(0f, 10f)]
        private float intensity = 1f;
        [SerializeField]
        private float downRate = 1f;
        [SerializeField]
        private float upRate = 0.5f;

        private RenderTexture[] rts = new RenderTexture[MaxIteration];


        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(this.validity == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            var width = source.width;
            var height = source.height;

            var i = 0;
            this.material.SetFloat(PropDelta, this.downRate);
            this.material.SetFloat(PropIntensity, this.intensity);
            this.material.SetVector(PropFilters, this.GetSoftKneeFilters());
            this.material.SetTexture(PropSourceTex, source);

            // Step downsampling.
            for(i = 0; i < this.iteration; i++)
            {
                width /= 2;
                height /= 2;
                if(width < 2 || height < 2)
                {
                    break;
                }

                rts[i] = RenderTexture.GetTemporary(width, height, 0, source.format);
                Graphics.Blit(i == 0 ? source : rts[i - 1], rts[i], this.material, i == 0 ? 0 : 1);
            }

            this.material.SetFloat(PropDelta, this.upRate);
            // Step upsampling.
            for(i -= 1; i > 0; i--)
            {
                Graphics.Blit(rts[i], rts[i - 1], this.material, 1);
                rts[i].Release();
            }

            Graphics.Blit(rts[0], destination, this.material, 2);
            rts[0].Release();
        }

        private Vector4 GetSoftKneeFilters()
        {
            var knee = this.threshold * this.softThreshold;
            return new Vector4()
            {
                x = this.threshold,
                y = this.threshold + knee,
                z = 2f * knee,
                w = 1f / (4f * (knee + float.MinValue))
            };
        }
    }
}
