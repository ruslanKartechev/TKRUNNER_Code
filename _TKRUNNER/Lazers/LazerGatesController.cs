using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
namespace TKRunner
{



    public class LazerGatesController : MonoBehaviour
    {
        public LazerGatesData Data;
        [SerializeField] private LazerGatesManager manager;
        [SerializeField] private LazerBeam beam;

        private void Start()
        {
            GameManager.Instance._events.LevelLoaded.AddListener(OnLevelLoaded);   
        }
        private void OnLevelLoaded()
        {
            manager.Init(this, beam);
            ActivateLazer();
        }
        public void ActivateLazer()
        {
            manager.StartLazer();
            manager.StartMovement();
        }
        public void DeactivateLazer()
        {
            manager.StopLazer();
            manager.StopMovement();
        }
    }
}