using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace TKRunner
{
    public class ProceduralSpawnerPositioner : MonoBehaviour
    {
        private Coroutine following;
        private SplineComputer _spline;
        [Header("How much ahead of camera spline projection")]
        [SerializeField] private float AddPercent = 3f;
        public void Init(SplineComputer spline)
        {
            _spline = spline;
            Stop();
            following = StartCoroutine(Following());
        }

        public void Stop()
        {
            if (following != null) StopCoroutine(following);

        }

        private IEnumerator Following()
        {
            while (_spline != null)
            {
                SplineSample sample = _spline.Project(Camera.main.transform.position);
                SplineSample ss = _spline.Evaluate(sample.percent + AddPercent / 100);
                transform.position = ss.position;
                transform.rotation = Quaternion.LookRotation(ss.forward);
                yield return null;
            }
        }
    }
}