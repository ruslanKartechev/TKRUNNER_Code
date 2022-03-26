using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
namespace TKRunner
{
    public class DragConstraintManager
    {

        private SplineComputer _centralSpline;
        private float _maxOffset = 5f;
        public float MaxOffset { get { return _maxOffset; }set{ _maxOffset = value; } }

        public void SetSpline(SplineComputer _spline)
        {
            _centralSpline = _spline;
        }
        public bool CheckConstraint(Vector3 newPos)
        {

           SplineSample res = _centralSpline.Project(newPos);
            Vector3 dir = newPos - res.position;
            float proj = Vector3.Dot(dir, res.right);
            if (Mathf.Abs(proj) < _maxOffset)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        

    }

}
