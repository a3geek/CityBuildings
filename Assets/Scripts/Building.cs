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

        public Vector2 BottomLeft
        {
            get { return new Vector2(this.MinX, this.MinZ); }
        }
        public Vector2 BottomRight
        {
            get { return new Vector2(this.MaxX, this.MinZ); }
        }
        public Vector2 TopLeft
        {
            get { return new Vector2(this.MinX, this.MaxZ); }
        }
        public Vector2 TopRight
        {
            get { return new Vector2(this.MaxX, this.MaxZ); }
        }

        public Vector2 Center;
        public Vector3 Size;


        public bool IsSuffer(Build other)
        {
            return other.MaxX <= this.MaxX && other.MinX >= this.MinX && other.MaxZ <= this.MaxZ && other.MinZ >= this.MinZ;
        }

        public bool IsContains(Vector2 point)
        {
            return point.x >= this.MinX && point.x <= this.MaxX && point.y >= this.MinZ && point.y <= this.MaxZ;
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

        var first = this.CreateBuild(ground);
        this.builds.Add(first);

        //var counter = 0;
        //do
        //{
        //    var build = this.CreateBuild(groundSize);

        //    counter++;
        //    for(var i = 0; i < this.builds.Count; i++)
        //    {
        //        var b = this.builds[i];

        //        var min = new Vector2(b.MinX - ground.MinX, b.MinZ - ground.MinZ);
        //        var max = new Vector2(ground.MaxX - b.MaxX, ground.MaxZ - b.MaxZ);

        //        var isLeft = min.x > max.x;
        //        var isBototm = min.y > max.y;

        //        var randX = isLeft ? Random.Range(ground.MinX, b.MinX) : Random.Range(b.MaxX, ground.MaxX);
        //        var randY = isBototm ? Random.Range(ground.MinZ, b.MinZ) : Random.Range(b.MaxZ, ground.MaxZ);

        //        var half = new Vector2(build.Size.x, build.Size.z) * 0.5f;
        //        build.Center = new Vector2(
        //            randX - (isLeft ? -half.x : half.x),
        //            randY - (isBototm ? -half.y : half.y)
        //        );

        //        bool bl = false, br = false, tl = false, tr = false;
        //        for(var j = 0; j < this.builds.Count && !(bl & br & tl & tr); j = j + ((j + 1) == i ? 2 : 1))
        //        {
        //            bl = bl | this.builds[j].IsContains(build.BottomLeft);
        //            br = br | this.builds[j].IsContains(build.BottomRight);
        //            tl = tl | this.builds[j].IsContains(build.TopLeft);
        //            tr = tr | this.builds[j].IsContains(build.TopRight);
        //        }

        //        if((bl & br & tl & tr) == false)
        //        {
        //            this.builds.Add(build);


        //            this.builds.Add(ground);
        //            return;



        //            counter = 0;
        //            break;
        //        }
        //    }
        //}
        //while(counter < 5);

    }

    private Build CreateBuild(Vector2 groundSize)
    {
        var rate = new Vector2(Random.Range(0.5f, 0.75f), Random.Range(0.5f, 0.75f));

        var size = new Vector2(
            groundSize.x * rate.x,
            groundSize.y * rate.y
        );

        return new Build()
        {
            Center = Vector3.zero,
            Size = new Vector3(size.x, Random.value * this.mainHeight, size.y)
        };
    }

    private Build CreateBuild(Build ground)
    {
        var build = this.CreateBuild(new Vector2(ground.Size.x, ground.Size.z));

        var half = new Vector2(build.Size.x, build.Size.z) * 0.5f;
        build.Center = new Vector2(
            Random.Range(ground.MinX + half.x, ground.MaxX - half.x),
            Random.Range(ground.MinZ + half.y, ground.MaxZ - half.y)
        );

        return build;
    }

    private void OnDrawGizmos()
    {
        if(this.builds.Count <= 2)
        {
            return;
        }

        this.BuildGizmos(this.builds[0], Color.white);
        this.BuildGizmos(this.builds[this.builds.Count - 1], Color.red);
        
        for(var i = 1; i < this.builds.Count - 1; i++)
        {
            this.BuildGizmos(this.builds[i], Color.black);
        }
    }

    private void BuildGizmos(Build build, Color color)
    {
        var y = build.Size.y * 0.5f;

        Gizmos.color = color;
        Gizmos.DrawWireCube(new Vector3(build.Center.x, y, build.Center.y), new Vector3(build.Size.x, 0.1f, build.Size.z));

        //var y = build.Size.y * 0.5f;

        //Gizmos.color = Color.black;
        //Gizmos.DrawCube(
        //    new Vector3(build.Center.x, y, build.Center.y),
        //    build.Size
        //);

        //Gizmos.color = Color.white;
        //Gizmos.DrawWireCube(
        //    new Vector3(build.Center.x, y, build.Center.y),
        //    build.Size
        //);
    }
}
