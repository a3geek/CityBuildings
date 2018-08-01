using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Components
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Window Texture")]
    public class WindowTexture : MonoBehaviour
    {
        public const string PropRandSeed = "randSeed";
        public const string PropWindowTex = "windowTex";
        public const string PropNoiseFrequency = "noiseFrequency";
        public const int ThreadX = 8;
        public const int ThreadY = 8;

        public RenderTexture Tex
        {
            get; private set;
        }

        [SerializeField]
        private ComputeShader cs = null;
        [SerializeField]
        private int width = 1024;
        [SerializeField]
        private int height = 1024;
        [SerializeField]
        private float noiseFrequency = 1f;

        [Header("Debug")]
        [SerializeField]
        private Renderer render = null;


        public void Init()
        {
            this.width = Mathf.IsPowerOfTwo(this.width) == false ? Mathf.NextPowerOfTwo(this.width) : this.width;
            this.height = Mathf.IsPowerOfTwo(this.height) == false ? Mathf.NextPowerOfTwo(this.height) : this.height;

            this.Tex = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true
            };
            this.Tex.Create();

            this.cs.SetInt(PropRandSeed, Mathf.Abs(Random.Range(0, int.MaxValue)));
            this.cs.SetFloat(PropNoiseFrequency, this.noiseFrequency);
            this.cs.SetTexture(0, PropWindowTex, this.Tex);

            this.cs.Dispatch(0, this.width / ThreadX, this.height / ThreadY, 1);

            this.render.material.mainTexture = this.Tex;
        }
    }
}
