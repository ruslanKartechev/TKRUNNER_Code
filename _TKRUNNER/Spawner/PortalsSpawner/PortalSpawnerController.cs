using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Commongame.Data;

namespace TKRunner
{

    public class PortalSpawnerController : MonoBehaviour, ISpawner
    {

        [Header("Settings")]
        public float spawnDelay;
        public GameObject PF;
        public SpawnMode _SpawnMode;
        public ActivatorModes _ActivationMode;
        [Space(10)]
        [SerializeField] private List<PortalSpawnerManager> _spawners = new List<PortalSpawnerManager>();
        public List<PortalSpawnerManager> _Spawners { get { return _spawners; } }

        private Action _Spawn;
        private Action<GameObject> onEnemySpawn = null;


        private void Start()
        {
            GetSpawners();
            Init();

        }

        public void Init()
        {
            switch (_SpawnMode)
            {
                case SpawnMode.AllInOne:
                    _Spawn = AllInOneSpawning;
                    break;
                case SpawnMode.OneByOne:
                    _Spawn = OneByOneSpawning;
                    break;
            }
            foreach (PortalSpawnerManager spawner in _spawners)
            {
                if(spawner != null)
                {
                    spawner.Init(_ActivationMode);
                    spawner.SetOnSpawnEvent(onEnemySpawn);
                    spawner.PreIsnt(PF);
                }

            }
        }


        public void SetActivationMode(ActivatorModes activationMode) => _ActivationMode = activationMode;


        public void Spawn()
        {
            _Spawn?.Invoke();
        }
        // Also reshaffles
        public void SetSpawnCount(int count)
        {
            int i = count;
            _spawners.Shuffle<PortalSpawnerManager>();
            foreach (PortalSpawnerManager spawner in _spawners)
            {
                spawner.SetSpawnCount(0);
            }
            while(i > 0)
            {
                foreach (PortalSpawnerManager spawner in _spawners)
                {
                    if (i > 0)
                    {
                        spawner._SpawnCount++;
                        i--;
                    }
                    else
                        break;
                }
            }
        
        }
        public void ReShaffleSpawners()
        {
            
        }
        public void SetOnSpawnEvent(Action<GameObject> onSpawn) => onEnemySpawn = onSpawn;

        public void SetEqualSpawnCount(int count)
        {
            foreach (PortalSpawnerManager spawner in _spawners)
            {
                spawner.SetSpawnCount(count);
            }
        }


        private void OneByOneSpawning()
        {
            foreach (PortalSpawnerManager spawner in _spawners)
            {
                if(spawner != null)
                    spawner.Spawn(PF,spawnDelay);
            }
        }

        private void AllInOneSpawning()
        {
            foreach (PortalSpawnerManager spawner in _spawners)
            {
                spawner.Spawn(PF, spawnDelay);
            }
        }

        public void GetSpawners()
        {
            _spawners = new List<PortalSpawnerManager>();

            for (int i = 0; i < transform.childCount; i++)
            {
                PortalSpawnerManager spawner = transform.GetChild(i).gameObject.GetComponent<PortalSpawnerManager>();
                if (spawner != null && _spawners.Contains(spawner) == false)
                    _spawners.Add(spawner);
            }

            if(transform.parent == null) return;
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                PortalSpawnerManager spawner = transform.parent.GetChild(i).gameObject.GetComponent<PortalSpawnerManager>();
                if (spawner != null && _spawners.Contains(spawner) == false)
                    _spawners.Add(spawner);
            }
            
        }



    }
    public static class ListExtention
    {

        private static System.Random rng = new System.Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }



#if UNITY_EDITOR

    [CustomEditor(typeof(PortalSpawnerController))]
    public class PortalSpawnerControllerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PortalSpawnerController me = (PortalSpawnerController)target;

            if (GUILayout.Button("GetSpawners"))
            {
                me.GetSpawners();
            }
        }
    }

#endif
}