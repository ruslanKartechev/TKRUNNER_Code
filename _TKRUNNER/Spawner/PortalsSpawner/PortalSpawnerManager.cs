using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using Commongame.Beizer;
using Commongame.Data;
using Commongame;
namespace TKRunner
{

    public class PortalSpawnerManager : MonoBehaviour
    {
        [Header("Dummy settings")]
        [SerializeField] private EnemySettings DummySettings;
        [Header("Jump Points")]
        [SerializeField] private BezierCurve curve;
        [SerializeField] private float SpawnRad;
        [SerializeField] private float JumpTime = 0.3f;
        [Space(5)]
        public int _SpawnCount = 1;

        private List<SpawnableData> preInst = new List<SpawnableData>();
        private bool didPreInst = false;

        [SerializeField ]private PortalSpawnerEffects _effects;

        public ActivatorModes _mode;
        private Action<GameObject> onEnemySpawn;

        public void Init(ActivatorModes mode)
        {
            _effects.Hide();
            _mode = mode;
        }

        public void SetOnSpawnEvent(Action<GameObject> onSpawn) => onEnemySpawn = onSpawn;
        public void SetSpawnCount(int count) => _SpawnCount = count;

        public void PreIsnt(GameObject prefab)
        {
            preInst = new List<SpawnableData>(_SpawnCount);
            for (int i = 0; i < _SpawnCount; i++)
            {
                GameObject go = Instantiate(prefab);
                ISpawnable spawnable = go.GetComponent<ISpawnable>();
                spawnable.SetSettings(DummySettings);
                preInst.Add(new SpawnableData(spawnable,go));
                spawnable.Hide();
            }
            didPreInst = true;
        }


        public async void Spawn(GameObject prefab, float delay)
        {
            _effects.FadeIn();
            if (didPreInst == true && preInst.Count >= _SpawnCount)
            {
                for(int i =0; i < preInst.Count; i++)
                {
                    preInst[i].gameObj.SetActive(true);
                    preInst[i].spawnable.OnSpawn();
                    JumpSpawn(preInst[i].gameObj.transform, JumpTime, preInst[i].spawnable);
                    await Task.Delay((int)(delay*1000));
                    preInst[i].spawnable.Activate(_mode);
                }
                didPreInst = false;
            }
            else
            {
                for (int i = 0; i < _SpawnCount; i++)
                {
                    GameObject go = Instantiate(prefab);
                    go.transform.parent = GameManager.Instance.data.currentInst.transform;
                    ISpawnable spawnable = go.GetComponent<ISpawnable>();
                    spawnable.SetSettings(DummySettings);
                    spawnable.OnSpawn();
                    JumpSpawn(go.transform, JumpTime,spawnable) ;
                    await Task.Delay((int)(delay * 1000));
                }
            }
            _effects.FadeOut();
        }

        private async void JumpSpawn(Transform target, float time, ISpawnable spawnable)
        {

            Vector3 endOffset = UnityEngine.Random.onUnitSphere;
            endOffset.y = 0;
            endOffset *= SpawnRad;

            List<Vector3> curvePoints = new List<Vector3>(3);
            curvePoints.Add(curve.GetPosLocal(0));
            curvePoints.Add(curve.GetPosLocal(1));
            curvePoints.Add( curve.GetPosLocal(2) + endOffset);

            float elapsed = 0f;
            target.position = curve.GetPoint(curvePoints,0f);
            while(elapsed <= time && target !=null)
            {
                target.position = curve.GetPoint(curvePoints, elapsed / time);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            target.position = curve.GetPoint(curvePoints,1f);
            spawnable.Activate(_mode);
            onEnemySpawn?.Invoke(target.gameObject);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

    }
}