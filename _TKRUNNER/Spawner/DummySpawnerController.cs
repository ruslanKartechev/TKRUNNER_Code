using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;
using Dreamteck.Splines;
namespace TKRunner
{

    public class DummySpawnerController : MonoBehaviour
    {
        [SerializeField] private DummySpawner spawner;
        private SpawningData data;
        public SpawningData Data { get { return data; } }

        public void Init()
        {
          //  spawner.Init(this,GameManager.Instance.dummyController);
          //  GameManager.Instance.eventManager.LevelLoaded.AddListener(OnNewLevel);
            //GameManager.Instance.eventManager.LevelStarted.AddListener(SpawnOnStart);
        }
        private void OnNewLevel()
        {
            //data = GameManager.Instance.data.currentInst.Data.spawningData;
            //if(data.SpawnDummiesOnStart)
            //    spawner.SpawnOnStart(GameManager.Instance.data.currentInst.startPoint);

        }
        public void SpawnOnStart()
        {
    
        }
        public void SpawnOnCP(SplineProjector splineProj, float interval,bool accelerate)
        {
           // spawner.SpawnOnCP(splineProj,interval,accelerate);            
        }
    }
}