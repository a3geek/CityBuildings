using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LineStructures.Examples
{
    using Components;

    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public sealed class LinesChecker : MonoBehaviour
    {
        public bool Execute = true;
        public float GizmosRadius = 10f;
        public Color GizmosColor = Color.white;

        [Space]
        [SerializeField]
        private int segment = 0;
        [SerializeField]
        private LineSegmentChecker segmentPrefab = null;

        [Space]
        [SerializeField]
        private int straight = 0;
        [SerializeField]
        private StraightLineChecker straightPrefab = null;

        [Space]
        [SerializeField]
        private int half = 0;
        [SerializeField]
        private HalfStraightLineChecker halfPrefab = null;

        private Stack<LineSegmentChecker> segments = new Stack<LineSegmentChecker>();
        private Stack<StraightLineChecker> straights = new Stack<StraightLineChecker>();
        private Stack<HalfStraightLineChecker> halfs = new Stack<HalfStraightLineChecker>();


        private void Update()
        {
            if(this.Execute == false)
            {
                return;
            }

            this.CheckObjs();
        }

        private void OnDrawGizmos()
        {
            if(this.Execute == false)
            {
                return;
            }

            var c = Gizmos.color;
            var checkers = this.segments.ToList().ConvertAll(s => (AbstractLineChecker)s)
                .Concat(this.straights.ToList().ConvertAll(s => (AbstractLineChecker)s))
                .Concat(this.halfs.ToList().ConvertAll(h => (AbstractLineChecker)h))
                .ToList();

            Gizmos.color = this.GizmosColor;
            for(var i = 0; i < checkers.Count; i++)
            {
                var c1 = checkers[i];

                for(var j = i + 1; j < checkers.Count; j++)
                {
                    var c2 = checkers[j];

                    if(Lines.IsCrossing(c1.Line, c2.Line) == true)
                    {
                        c1.IsCrossing = c2.IsCrossing = true;

                        var p = Lines.GetIntersection(c1.Line, c2.Line);
                        Gizmos.DrawSphere(new Vector3(p.x, 0f, p.y), this.GizmosRadius);
                    }
                }
            }

            Gizmos.color = c;
        }

        private void CheckObjs()
        {
            this.CheckObjs(this.segments, this.segment, this.segmentPrefab);
            this.CheckObjs(this.straights, this.straight, this.straightPrefab);
            this.CheckObjs(this.halfs, this.half, this.halfPrefab);
        }

        private void CheckObjs<T>(Stack<T> stack, int count, T prefab) where T : MonoBehaviour
        {
            if(stack.Count == 0 && count > 0)
            {
                foreach(var c in GetComponentsInChildren<T>())
                {
                    stack.Push(c);
                }
            }

            // Create.
            for(var i = count - stack.Count; i > 0; i--)
            {
                var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
                stack.Push(obj);
            }

            // Remove.
            for(var i = stack.Count - count; i < 0; i++)
            {
                var obj = stack.Pop();
                DestroyImmediate(obj.gameObject);
            }
        }
    }
}
