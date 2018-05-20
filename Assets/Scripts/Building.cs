using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {
    [System.Serializable]
    public class Build
    {
        public float x;
        public float y;

        public float sx;
        public float sy;
        public float sh;
    }

    public float centerX = 0f;
    public float centerY = 0f;
    public float fieldX = 10f;
    public float fieldY = 10f;

    public List<Build> builds = new List<Build>();


    private void Awake()
    {
        var sx = Random.Range(this.fieldX * 0.6f, this.fieldX * 0.8f);
        var sy = Random.Range(this.fieldY * 0.6f, this.fieldY * 0.8f);

        var x = this.centerX + (Random.value > 0.5f ? 1f : -1f) * Random.Range(0f, (this.fieldX - sx) / 2f);
        var y = this.centerY + (Random.value > 0.5f ? 1f : -1f) * Random.Range(0f, (this.fieldY - sy) / 2f);

        this.builds.Add(new Build()
        {
            x = x,
            y = y,
            sx = sx,
            sy = sy,
            sh = Random.Range(40f, 50f)
        });


        // bottom
        var d = (y - sy * 0.5f) - (this.centerY - this.fieldY * 0.5f);
        if(d > sy * 0.2f)
        {
            var bx = Random.Range(sx * 0.2f, sx * 0.5f);

            this.builds.Add(new Build()
            {
                x = x + (Random.value > 0.5f ? 1f : -1f) * Random.Range(0f, bx * 0.5f),
                y = y - Random.Range(sy * 0.1f, d),
                sx = bx,
                sy = sy,
                sh = Random.Range(15f, 40f)
            });
        }

        // top
        d = (this.centerY + this.fieldY * 0.5f) - (y + sy * 0.5f);
        if(d > sy * 0.2f)
        {
            var bx = Random.Range(sx * 0.2f, sx * 0.5f);

            this.builds.Add(new Build()
            {
                x = x + (Random.value > 0.5f ? 1f : -1f) * Random.Range(0f, bx * 0.5f),
                y = y + Random.Range(sy * 0.1f, d),
                sx = bx,
                sy = sy,
                sh = Random.Range(15f, 40f)
            });
        }

        // left
        d = (x - sx * 0.5f) - (this.centerX - this.fieldX * 0.5f);
        if(d > sx * 0.2f)
        {
            var by = Random.Range(sy * 0.2f, sy * 0.5f);

            this.builds.Add(new Build()
            {
                x = x - Random.Range(sx * 0.1f, d),
                y = y + (Random.value > 0.5f ? 1f : -1f) * Random.Range(0f, by * 0.5f),
                sx = sx,
                sy = by,
                sh = Random.Range(15f, 40f)
            });
        }

        // right
        d = (this.centerX + this.fieldX * 0.5f) - (x + sx * 0.5f);
        if(d > sx * 0.2f)
        {
            var by = Random.Range(sy * 0.2f, sy * 0.5f);

            this.builds.Add(new Build()
            {
                x = x + Random.Range(sx * 0.1f, d),
                y = y + (Random.value > 0.5f ? 1f : -1f) * Random.Range(0f, by * 0.5f),
                sx = sx,
                sy = by,
                sh = Random.Range(15f, 40f)
            });
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(
            new Vector3(this.centerX, 0f, this.centerY),
            new Vector3(this.fieldX, 0f, this.fieldY)
        );

        if(this.builds.Count <= 0)
        {
            return;
        }

        Gizmos.color = Color.blue;
        var b = this.builds[0];
        Gizmos.DrawWireCube(
            new Vector3(b.x, 0f, b.y),
            new Vector3(b.sx, 0f, b.sy)
        );
        Gizmos.DrawCube(new Vector3(b.x, 0f, b.y), new Vector3(0.5f, 0.1f, 0.5f));

        Gizmos.color = Color.red;
        foreach(var bb in this.builds)
        {
            Gizmos.DrawWireCube(
                new Vector3(bb.x, 0f, bb.y),
                new Vector3(bb.sx, 0f, bb.sy)
            );
        }
    }
}
