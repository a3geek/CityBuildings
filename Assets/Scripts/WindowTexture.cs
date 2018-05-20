using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowTexture : MonoBehaviour
{
    public const string RandSeedParam = "rand_seed";
    public const string WindowTexParam = "window_tex";
    public const int ThreadX = 8;
    public const int ThreadY = 8;

    public RenderTexture RT
    {
        get; private set;
    }
    public Renderer render;

    [SerializeField]
    private ComputeShader cs;
    [SerializeField]
    private int width = 1024;
    [SerializeField]
    private int height = 1024;


    private void Start()
    {
        this.width = Mathf.IsPowerOfTwo(this.width) == false ? Mathf.NextPowerOfTwo(this.width) : this.width;
        this.height = Mathf.IsPowerOfTwo(this.height) == false ? Mathf.NextPowerOfTwo(this.height) : this.height;

        this.RT = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true
        };
        this.RT.Create();

        this.cs.SetInt(RandSeedParam, Mathf.Abs(Random.Range(0, int.MaxValue)));
        this.cs.SetTexture(0, WindowTexParam, this.RT);

        this.cs.Dispatch(0, this.width / ThreadX, this.height / ThreadY, 1);
        this.render.material.mainTexture = this.RT;
    }
}
