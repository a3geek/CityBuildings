using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuildings.Managers
{
    [DisallowMultipleComponent]
    [AddComponentMenu("City Buildings/Managers/Window Texture Manager")]
    public class WindowTextureManager : MonoBehaviour
    {
        public const string PropRandSeed = "randSeed";
        public const string PropWindowTex = "windowTex";
        public const string PropNoiseFrequency = "noiseFrequency";
        public const string PropWallColor = "wallColor";
        public const string PropMainColor = "mainColor";
        public const int ThreadX = 8;
        public const int ThreadY = 8;

        public RenderTexture NightTex
        {
            get; private set;
        }
        public RenderTexture NoonTex
        {
            get; private set;
        }
        public RenderTexture BuildScalerTex
        {
            get; private set;
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
        private Color nightWall = Color.black;
        [SerializeField]
        private Color nightMain = Color.black;
        [SerializeField]
        private Color noonWall = Color.gray;
        [SerializeField]
        private Color noonMain = Color.blue;
        [SerializeField]
        private int buildScalerWindowNum = 5;
        

        public void Init()
        {
            this.width = Mathf.IsPowerOfTwo(this.width) == false ? Mathf.NextPowerOfTwo(this.width) : this.width;
            this.height = Mathf.IsPowerOfTwo(this.height) == false ? Mathf.NextPowerOfTwo(this.height) : this.height;

            this.BuildScalerTex = this.GetRenderTexture(ThreadX, ThreadY * this.buildScalerWindowNum);

            this.cs.SetInt(PropRandSeed, Mathf.Abs(Random.Range(0, int.MaxValue)));
            this.cs.SetFloat(PropNoiseFrequency, this.noiseFrequency);
            this.cs.SetTexture(1, PropWindowTex, this.BuildScalerTex);

            this.cs.Dispatch(1, 1, this.buildScalerWindowNum, 1);
        }

        public void InitNight()
        {
            this.NightTex = this.GetRenderTexture(this.width, this.height);

            this.cs.SetVector(PropWallColor, this.nightWall);
            this.cs.SetVector(PropMainColor, this.nightMain);
            this.cs.SetTexture(0, PropWindowTex, this.NightTex);

            this.cs.Dispatch(0, this.width / ThreadX, this.height / ThreadY, 1);
        }

        public void InitNoon()
        {
            this.NoonTex = this.GetRenderTexture(this.width, this.height);

            this.cs.SetVector(PropWallColor, this.noonWall);
            this.cs.SetVector(PropMainColor, this.noonMain);
            this.cs.SetTexture(0, PropWindowTex, this.NoonTex);

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
