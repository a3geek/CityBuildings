using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CityBuildings.Managers
{
    using Random = UnityEngine.Random;

    [DisallowMultipleComponent]
    [AddComponentMenu("City Buildings/Managers/Window Texture Manager")]
    public class WindowTextureManager : MonoBehaviour
    {
        [Serializable]
        private class TimeColors
        {
            public Color Wall = Color.black;
            public Color Main = Color.black;

            [HideInInspector]
            public RenderTexture Texture = null;
        }

        public const string PropRandSeed = "randSeed";
        public const string PropWindowTex = "windowTex";
        public const string PropNoiseFrequency = "noiseFrequency";
        public const string PropWallColor = "wallColor";
        public const string PropMainColor = "mainColor";
        public const int ThreadX = 8;
        public const int ThreadY = 8;

        public RenderTexture this[int index]
        {
            get
            {
                return this.timeColors[index].Texture;
            }
        }

        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }
        public Vector2Int WindowNumber
        {
            get { return new Vector2Int(this.width / ThreadX, this.height / ThreadY); }
        }

        [SerializeField]
        private ComputeShader cs = null;
        [SerializeField]
        private int width = 1024;
        [SerializeField]
        private int height = 1024;
        [SerializeField]
        private float noiseFrequency = 1f;
        [SerializeField]
        private List<TimeColors> timeColors = new List<TimeColors>();
        

        public void Initialize(int index)
        {
            if(index == 0)
            {
                this.width = Mathf.IsPowerOfTwo(this.width) == false ? Mathf.NextPowerOfTwo(this.width) : this.width;
                this.height = Mathf.IsPowerOfTwo(this.height) == false ? Mathf.NextPowerOfTwo(this.height) : this.height;

                this.cs.SetInt(PropRandSeed, Mathf.Abs(Random.Range(0, int.MaxValue)));
                this.cs.SetFloat(PropNoiseFrequency, this.noiseFrequency);
            }

            var colors = this.timeColors[index];
            colors.Texture = this.GetRenderTexture(this.width, this.height);

            this.cs.SetVector(PropWallColor, colors.Wall);
            this.cs.SetVector(PropMainColor, colors.Main);
            this.cs.SetTexture(0, PropWindowTex, colors.Texture);

            this.cs.Dispatch(0, this.width / ThreadX, this.height / ThreadY, 1);
        }
        
        private RenderTexture GetRenderTexture(int width, int height)
        {
            var rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
            {
                enableRandomWrite = true,
                useMipMap = false,
                filterMode = FilterMode.Point,
                anisoLevel = 0,
                wrapMode = TextureWrapMode.Repeat,
                autoGenerateMips = false
            };
            rt.Create();

            return rt;
        }
    }
}
