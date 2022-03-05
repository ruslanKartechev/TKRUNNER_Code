using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using General.Data;
using Dreamteck.Splines;
using TKRunner;
namespace General
{
    public class LevelInstance : MonoBehaviour
    {
        public SplineComputer levelSpline;
        public SplineComputer playerSpline;
        public DataGameMain Data;
        //  public SpawnCheckpoint startPoint;
        [Header("Camera")]
        public Transform followObj;
        public Transform lookObj;
        public Transform EndPos;


       [HideInInspector] public float TrackUnitLength;
        private void Start()
        {
            if (levelSpline == null)
                Debug.Log("Track spline not assined");
            TrackUnitLength = levelSpline.CalculateLength() / 100;

        }
    }
}