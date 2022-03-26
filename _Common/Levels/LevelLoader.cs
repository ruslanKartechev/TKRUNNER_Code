using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
using System.Collections;

namespace Commongame
{
    public class LevelLoader : MonoBehaviour
    {
        private LevelManager manager;
        public Transform levelPoint;
        public void Init(LevelManager _manager)
        {
            manager = _manager;
            if (levelPoint == null)
                levelPoint = transform;
        }
        public void Load(LevelData data)
        {
            StartCoroutine(Loading(data));
        }

        private IEnumerator Loading(LevelData data)
        {
            GameObject level = Instantiate(data.lvlPF, levelPoint);
            LevelInstance currentData = level.GetComponent<LevelInstance>();
            GameManager.Instance.data.currentInst = currentData;
            yield return null;
            // GameManager.Instance.playerSpawner.SpawnPlayer();
           // yield return null;
            GameManager.Instance.eventManager.LevelLoaded.Invoke();

        }

        public void ClearLevel()
        {

            for (int i = 0; i < levelPoint.childCount; i++)
            {
                GameObject destroyObject = levelPoint.GetChild(i).gameObject;
                DestroyImmediate(destroyObject);
            }
            
        }
    }
}



