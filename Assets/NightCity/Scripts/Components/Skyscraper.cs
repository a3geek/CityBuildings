using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Skyscraper : MonoBehaviour
{
    [System.Serializable]
    public struct Build
    {
        public float MaxX
        {
            get { return this.center.x + this.size.x * 0.5f; }
        }
        public float MinX
        {
            get { return this.center.x - this.size.x * 0.5f; }
        }
        public float MaxZ
        {
            get { return this.center.y + this.size.z * 0.5f; }
        }
        public float MinZ
        {
            get { return this.center.y - this.size.z * 0.5f; }
        }

        public Vector3 center;
        public Color color;
        public Vector3 size;
        public Vector3 baseSize;
    }

    public float groundHeight = 1f;
    public float groundX = 10f;
    public float groundY = 10f;
    public float groundRateX = 0.5f;
    public float groundRateY = 0.5f;

    [Space]
    public float mainHeight = 25f;
    [Space]
    public List<Build> builds = new List<Build>();
    [Space]
    public Material mat = null;

    private ComputeBuffer buffer = null;


    private void Awake()
    {
        //this.CreateBuild(Vector3.zero);
        //this.CreateBuild(new Vector3(this.groundX + 5f, 0f, 0f));

        this.builds.Add(new Build
        {
            center = new Vector3(0f, 0.5f, 0f),
            color = Color.black,
            size = new Vector3(5f, 10f, 5f),
            baseSize = new Vector3(10f, 1f, 10f)
        });

        this.builds.Add(new Build
        {
            center = new Vector3(20f, 0.5f, 0f),
            color = Color.black,
            size = new Vector3(5f, 10f, 5f),
            baseSize = new Vector3(10f, 1f, 10f)
        });
    }
    
    private void Start()
    {
        this.buffer = new ComputeBuffer(this.builds.Count, Marshal.SizeOf(typeof(Build)), ComputeBufferType.Default);
        this.buffer.SetData(this.builds.ToArray());
    }

    private void OnRenderObject()
    {
        this.mat.SetPass(0);
        this.mat.SetBuffer("_data", this.buffer);

        Graphics.DrawProcedural(MeshTopology.Points, 1, this.buffer.count);
    }

    private void OnDestroy()
    {
        this.buffer.Release();
    }

    private void CreateBuild(Vector3 offset)
    {
        // ground
        var groundSize = new Vector3(
            this.groundX * Random.Range(this.groundRateX, 1f),
            this.groundHeight,
            this.groundY * Random.Range(this.groundRateY, 1f)
        );
        var ground = new Build()
        {
            center = offset,
            size = groundSize
        };

        var first = this.CreateBuild(ground);

        this.builds.Add(ground);
        this.builds.Add(first);
    }

    private Build CreateBuild(Build ground)
    {
        var rate = new Vector2(Random.Range(0.5f, 0.75f), Random.Range(0.5f, 0.75f));

        var size = new Vector3(
            ground.size.x * rate.x,
            Random.value * this.mainHeight,
            ground.size.y * rate.y
        );

        var half = size * 0.5f;
        return new Build()
        {
            size = size,
            center = (new Vector3(
                Random.Range(ground.MinX + half.x, ground.MaxX - half.x),
                half.y,
                Random.Range(ground.MinZ + half.z, ground.MaxZ - half.z)
            ))
        };
    }
}
