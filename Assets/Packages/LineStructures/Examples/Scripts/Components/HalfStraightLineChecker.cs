using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures.Examples.Components
{
    using LineStructures.Components;

    [AddComponentMenu("")]
    public class HalfStraightLineChecker : AbstractLineChecker
    {
        public override ILine Line
        {
            get { return this.line; }
        }
        public HalfStraightLine HalfStraightLine
        {
            get { return this.line; }
        }
        
        [Header("Half Straight Line Params")]
        public float Magnitude = 500f;

        private HalfStraightLine line = new HalfStraightLine();


        protected override void UpdateLine()
        {
            this.p0.name = "p";
            this.p1.name = "dir";
            
            var pre = new Vector3(this.line.p.x, 0f, this.line.p.y);
            var curr = this.p0.transform.position;

            this.line.p = this.ToV2FromV3(this.p0.transform.position);

            this.p1.transform.position += (curr - pre);
            this.line.dir = this.ToV2FromV3(this.p1.transform.position - this.p0.transform.position);
            this.line.t = this.Magnitude;
        }

        protected override void DrawGizmos()
        {
            this.line.DrawGizmos();
        }
    }
}
