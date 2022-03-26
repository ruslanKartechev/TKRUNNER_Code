using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Commongame;
using Commongame.Data;
using System.Threading;
using System.Threading.Tasks;

namespace TKRunner
{
    public class DummyController : MonoBehaviour
    {
        [SerializeField] private TriggerSpawnerManager triggerSpawner;
        [Space(5)]
        private List<DummyManager> spawnedDummies = new List<DummyManager>(50);
        private SpawningData Data;
        private void Start()
        {

            GameManager.Instance.eventManager.PlayerLose.AddListener(OnPlayerLose);
            GameManager.Instance.eventManager.PlayerWin.AddListener(OnPlayerWin);
            GameManager.Instance.eventManager.LevelLoaded.AddListener(OnNewLevel);
        }

        private void OnPlayerWin()
        {
            for (int i = 0; i < spawnedDummies.Count; i++)
            {
                if (spawnedDummies[i] != null && spawnedDummies[i].gameObject.activeInHierarchy == true)
                {
                    spawnedDummies[i].Defeated();
                }
            }
        }
        private void OnPlayerLose()  // Remove ?
        {
            for (int i = 0; i < spawnedDummies.Count; i++)
            {
                if (spawnedDummies[i] != null &&  spawnedDummies[i].gameObject.activeInHierarchy == true)
                {
                    spawnedDummies[i].Winner();
                }
            }

            triggerSpawner.SetSpline(GameManager.Instance.data.currentInst.levelSpline);
            triggerSpawner.SpawnOnPlayerDeath();

        }


        public void AddDummy(DummyManager dummy)
        {
            spawnedDummies.Add(dummy);
        }
        public void AddDummies(List<DummyManager> dummies)
        {
            spawnedDummies.AddRange(dummies);
        }
        public int DummiesCount()
        {
            int count = 0;
            foreach(DummyManager d in spawnedDummies)
            {
                if (d != null && d.gameObject.activeInHierarchy == true && d.CurrentState != DummyStates.Dead)
                    count++;
            }
            //Debug.Log("current count: " + count);
            return count;
        }


        private void OnNewLevel()
        {
            spawnedDummies = new List<DummyManager>(50);
        }


    }

}
