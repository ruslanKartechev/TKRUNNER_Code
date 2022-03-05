using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using General;
using General.Data;
using System.Threading;
using System.Threading.Tasks;

namespace TKRunner
{
    public class DummyController : MonoBehaviour
    {
        [SerializeField] private GameObject DummyPF;
        [SerializeField] private GameObject GuardedPF;
        [SerializeField] private DummyManager[] dummies;
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
        }


        public void AddDummy(DummyManager dummy)
        {
            spawnedDummies.Add(dummy);
        }
        public void AddDummies(List<DummyManager> dummies)
        {
            spawnedDummies.AddRange(dummies);
        }

        public async void StartAllDummies()
        {
            List<Task> tasks = new List<Task>(spawnedDummies.Count);
            for (int i = 0; i < spawnedDummies.Count; i++)
            {
                tasks.Add(spawnedDummies[i].StartMoving() );
            }
            await Task.WhenAll(tasks);
            tasks.Clear();
            for (int i = 0; i < spawnedDummies.Count; i++)
            {
                tasks.Add(spawnedDummies[i].Accelerate(Data.AccelerationTime, Data.StartMaxSpeed, Data.StartDuration));
            }
            await Task.WhenAll(tasks);
        }
        public void StopAllDummies()
        {
            for (int i = 0; i < spawnedDummies.Count; i++)
            {
                spawnedDummies[i].StopMoving();
            }
        }
        
        public DummyTarget GetClosestTarget()
        {
            float playerPercent = GameManager.Instance.data.Player.currentPercent;
            float shortest = 100000000000000f;
            DummyTarget aimTarget = null;
            foreach(DummyManager dummy in spawnedDummies)
            {
                if( dummy && dummy.gameObject.activeInHierarchy 
                   && (dummy.CurrentState == DummyStates.Run || dummy.CurrentState == DummyStates.Standup)
                   )
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(dummy.gameObject.transform.position);
                    float dist = (Input.mousePosition - screenPos).magnitude;
                    if(dist <= shortest)
                    {
                        shortest = dist;
                        aimTarget = dummy._Target;
                    }
                }
            }
            return aimTarget;
        }
        private void OnNewLevel()
        {
            for(int i = 0; i<spawnedDummies.Count; i++)
            {
                if (spawnedDummies[i] != null)
                    spawnedDummies[i].gameObject.SetActive(false);
            }

            spawnedDummies = new List<DummyManager>(50);
        }


    }

}
