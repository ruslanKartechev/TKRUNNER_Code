using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Commongame;
namespace TKRunner
{
    public class SplinePositionHandler : MonoBehaviour
    {
        private SplineComputer _spline;
        public void Init()
        {
            _spline = GameManager.Instance._data.currentInst.playerSpline;
            if (_spline == null) Debug.Log("Player spline was not found");

        }

        public double GetTargetPercent(Vector3 pos)
        {
            return _spline.Project(pos).percent;
        }
        public double GetPlayerPercent()
        {
            return GameManager.Instance._data.Player.currentPercent;
        }
    }
}