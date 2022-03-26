using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Commongame;

namespace TKRunner
{
    [System.Serializable]
    public enum WeaponType
    {
        Default, Hammer, Shotgun, Sword, Axe, Bat
    }
    public class ProceduralSpawnerManager : MonoBehaviour
    {

        private ISpawner _spawner;
        private SpawnSettingsbyType currentSettings;
        private List<SpawnSettingsbyType> _settings;

        private Coroutine _countdownHandler;
        private float countdown;
        private float currentDelay;

        private WeaponType currentType;
        private ProceduralSpawnerController _Controller;
        public void Init(List<SpawnSettingsbyType> settings, ProceduralSpawnerController controller,ISpawner spawner)
        {
            _settings = settings;
            _spawner = spawner;
            _Controller = controller;
            SetNewType(WeaponType.Default);
            countdown = _Controller.AfterStartDelay;

            Stop();
            _countdownHandler = StartCoroutine(SpawningCountdown());
        }

        public void Stop()
        {
            if (_countdownHandler != null)
                StopCoroutine(_countdownHandler);
        }

        public void ResetCountdown()
        {
            countdown = currentDelay;
        }

        public void SetNewType(WeaponType type)
        {
            currentSettings = _settings.Find(x => x.type == type);
            currentDelay = currentSettings.SpawningDelay;
            currentType = type;

        }

        public void SpawnByType(WeaponType type)
        {
            SetNewType(type);
            ResetCountdown();
            int count = UnityEngine.Random.Range(currentSettings.Count_min, currentSettings.Count_max);
            int spawned = GameManager.Instance.dummyController.DummiesCount();
            if (count + spawned >= _Controller.MaxSpawned) 
            {
                count = _Controller.MaxSpawned - spawned;
            }
            if (count > 0)
                Spawn(count);
        }
        
        public void Spawn(int count)
        {
            _spawner.SetSpawnCount(count);
            _spawner.Spawn();
        }

        private IEnumerator SpawningCountdown()
        {
            while (true)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0)
                {
                    SpawnByType(currentType);
                }
                yield return null;
            }
        }

    }
}