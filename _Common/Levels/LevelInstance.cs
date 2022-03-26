using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;
using Dreamteck.Splines;
using TKRunner;
namespace Commongame
{
    public class LevelInstance : MonoBehaviour
    {
        public SplineComputer levelSpline;
        public SplineComputer playerSpline;
        public float TrackHalfWidth = 5f;
        public DataGameMain Data;
        //  public SpawnCheckpoint startPoint;
        [Header("Camera")]
        public Transform followObj;
        public Transform lookObj;
        public Transform WinCamPos;
        public Transform LooseCam;


       [HideInInspector] public float TrackUnitLength;
        private void Start()
        {
            if (levelSpline == null)
                Debug.Log("Track spline not assined");
            TrackUnitLength = levelSpline.CalculateLength() / 100;

        }
    }
}