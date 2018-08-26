using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures.Examples.Components
{
    using LineStructures.Components;

    [AddComponentMenu("")]
    public class LineSegmentChecker : AbstractLineChecker
    {
        public override ILine Line
        {
            get { return this.line; }
        }
        public LineSegment LineSegment
        {
            get { return this.line; }
        }

        private LineSegment line = new LineSegment();


        protected override void UpdateLine()
        {
            this.line.p0 = this.ToV2FromV3(this.p0.transform.position);
            this.line.p1 = this.ToV2FromV3(this.p1.transform.position);
        }

        protected override void DrawGizmos()
        {
            this.line.DrawGizmos();
        }
    }
}
