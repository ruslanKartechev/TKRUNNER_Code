using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;

namespace TKRunner
{

    public class LazerSpinnerController : MonoBehaviour
    {
        [SerializeField] private List<LazerBeam> beams = new List<LazerBeam>();
        [SerializeField] private LazerSpinnerManager manager;
        public LazerSpinnerData Data;

        private void Start()
        {
            manager.Init(this, beams);
            StartLazer();
        }


        public void StartLazer()
        {
            manager.StartLazers();
            manager.StartMovement();
        }
        public void StopLazer()
        {
            manager.StopMovement();
            manager.StopLazers();
        }


    }
}