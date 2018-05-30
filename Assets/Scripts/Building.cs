using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [System.Serializable]
    public class Build
    {
        public float MaxX
        {
            get { return this.Center.x + this.Size.x * 0.5f; }
        }
        public float MinX
        {
            get { return this.Center.x - this.Size.x * 0.5f; }
        }
        public float MaxZ
        {
            get { return this.Center.y + this.Size.z * 0.5f; }
        }
        public float MinZ
        {
            get { return this.Center.y - this.Size.z * 0.5f; }
        }

        public Vector2 Center;
        public Vector3 Size;


        public bool IsSuffer(Build other)
        {
            return other.MaxX <= this.MaxX && other.MinX >= this.MinX && other.MaxZ <= this.MaxZ && other.MinZ >= this.MinZ;
        }
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


    private void Awake()
    {
        // ground
        var groundSize = new Vector2(
            this.groundX * Random.Range(this.groundRateX, 1f),
            this.groundY * Random.Range(this.groundRateY, 1f)
        );
        var ground = new Build()
        {
            Center = Vector2.zero,
            Size = new Vector3(groundSize.x, this.groundHeight, groundSize.y)
        };
        this.builds.Add(ground);
        
        do
        {
            var rate = new Vector2(Random.Range(0.5f, 0.75f), Random.Range(0.5f, 0.75f));

            var size = new Vector2(
                groundSize.x * rate.x,
                groundSize.y * rate.y
            );

            var half = size * 0.5f;
            var pos = new Vector2(
                Random.Range(ground.MinX + half.x, ground.MaxX - half.x),
                Random.Range(ground.MinZ + half.y, ground.MaxZ - half.y)
            );

            this.builds.Add(new Build()
            {
                Center = pos,
                Size = new Vector3(size.x, Random.value * this.mainHeight, size.y)
            });


            //var half = size * 0.5f;
            //var pos = new Vector2(Random.Range(0f, half.x), Random.Range(0f, half.y));

            //pos.x = Mathf.Clamp(pos.x, 0f, size.x);
            //pos.y = Mathf.Clamp(pos.y, 0f, size.y);

            //this.builds.Add(new Build()
            //{
            //    Center = new Vector2(pos.x * (Random.value < 0.5f ? -1 : 1), pos.y * (Random.value < 0.5f ? -1 : 1)),
            //    Size = new Vector3(size.x, Random.value * this.mainHeight, size.y)
            //});
        }
        while(false);
    }

    private void OnDrawGizmos()
    {
        if(this.builds.Count <= 0)
        {
            return;
        }

        for(var i = 0; i < this.builds.Count; i++)
        {
            this.BuildGizmos(this.builds[i]);
        }
    }

    private void BuildGizmos(Build build)
    {
        var y = build.Size.y * 0.5f;

        Gizmos.color = Color.black;
        Gizmos.DrawCube(
            new Vector3(build.Center.x, y, build.Center.y),
            build.Size
        );

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            new Vector3(build.Center.x, y, build.Center.y),
            build.Size
        );
    }
}
