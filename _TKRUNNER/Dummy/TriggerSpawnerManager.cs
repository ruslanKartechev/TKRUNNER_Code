using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;
using Dreamteck.Splines;
using System;
namespace TKRunner
{


    public class TriggerSpawnerManager : MonoBehaviour
    {
        [SerializeField]  private float PortalSpawnDistance = 10;
        [SerializeField] private int SpawnAmount = 3;

        [SerializeField] private GameObject SpawnerPF;
        [SerializeField] private List<IPlayerHunter> spawned = new List<IPlayerHunter>();

        public SplineComputer targetSpline;
        private List<Vector3> hunterPositions = new List<Vector3>();

        public void SetSpline(SplineComputer spline) => targetSpline = spline;

        public void SpawnOnPlayerDeath()
        {
            spawned = new List<IPlayerHunter>(SpawnAmount);
            GameObject s = Instantiate(SpawnerPF);
            PortalSpawnerController spawner = s.GetComponent<PortalSpawnerController>();
            double percent = targetSpline.Project(GameManager.Instance.data.Player.transform.position).percent;
            double percentDistance = PortalSpawnDistance / targetSpline.CalculateLength();
            Vector3 pos = targetSpline.EvaluatePosition(percent - percentDistance);
            s.transform.position = pos;
            SetEndPositions(SpawnAmount,GameManager.Instance.data.Player.GeometryCenter);

            spawner.SetActivationMode(ActivatorModes.Hunter);
            spawner.Init();
            spawner.SetEqualSpawnCount(SpawnAmount);
            spawner.SetOnSpawnEvent(OnEnemySpawn);
            
            spawner.Spawn();
        }
        private void SetEndPositions(int count, Transform target)
        {
            Transform player = GameManager.Instance.data.Player.transform;
            hunterPositions = new List<Vector3>(count);
            float rad = 1.3f;
            float angleSpacing = 360f / count;
            for(int i =0; i < count; i++)
            {
                Vector3 v = Quaternion.AngleAxis(angleSpacing * i, Vector3.up) * Vector3.forward * rad;
                v.y = player.position.y + 0.1f;
                hunterPositions.Add(v);
            }

        }
        public void OnEnemySpawn(GameObject enemy)
        {
            IPlayerHunter h = enemy.GetComponent<IPlayerHunter>();
            spawned.Add(h);
            h.InitHunter();
            h.SetTarget(GameManager.Instance.data.Player.GeometryCenter);
            h.SetSpline(targetSpline);

            Vector3 attackPoint = hunterPositions[0];
            h.SetAttackPoint(attackPoint);
            h.Attack();
            hunterPositions.RemoveAt(0);
        }


    }
}