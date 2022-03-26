using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using UnityEditor;
namespace TKRunner
{
    [System.Serializable]
    public class SpawnSettingsbyType
    {
        public WeaponType type;
        public float SpawningDelay = 5f;
        public int Count_min = 3;
        public int Count_max = 5;
    

    }


    public class ProceduralSpawnerController : MonoBehaviour
    {
        public ISpawner _spawner;
        [Header("Do not spawn, if n is already spawned")]
        public int MaxSpawned = 5;
        [Header("Spawn delay right after level starts")]
        public float AfterStartDelay = 1f;
        [Header("Spawn Settings By Type")]
        public List<SpawnSettingsbyType> _Settings = new List<SpawnSettingsbyType>();
        public ProceduralSpawnerManager _manager;
        public ProceduralSpawnerPositioner _positioner;

        private void Start()
        {
            _spawner = GetComponentInChildren<ISpawner>();
            
            if (_spawner == null) {Debug.Log("ISpawner not found"); return; }
            GameManager.Instance.eventManager.LevelStarted.AddListener(Init);
            GameManager.Instance.eventManager.LevelEndreached.AddListener(Stop);
            GameManager.Instance.eventManager.PlayerLose.AddListener(Stop);
            GameManager.Instance.eventManager.WeaponEquipped.AddListener(OnWeaponEquipped);
        }

        public void Init()
        {
            if (_manager == null) Debug.Log("procedural spawner manager not found");
            _manager.Init(_Settings,this, _spawner);
            _positioner?.Init(GameManager.Instance.data.currentInst.levelSpline);

        }

        public void Stop()
        {
            _manager.Stop();
        }
        public void OnWeaponEquipped()
        {
            WeaponType type = GameManager.Instance.data.currentWeapon;
            if (type != WeaponType.Default)
                _manager.SpawnByType(type);
            else
            {
                _manager.SetNewType(type);
                _manager.ResetCountdown();
            }
             
        }

    }


    [CustomEditor(typeof(ProceduralSpawnerController))]
    public class ProceduralSpawnerControllerEditor: Editor
    {
        //public override void OnInspectorGUI()
        //{
        //    base.OnInspectorGUI();
        //    ProceduralSpawnerController me = target as ProceduralSpawnerController;
        //    if (GUILayout.Button("GetSpawners"))
        //        me.GetSpawners();
        //}

    }
}
