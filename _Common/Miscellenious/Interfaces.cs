using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Dreamteck.Splines;
namespace TKRunner
{
    public interface IPlayerHunter
    {
        void InitHunter();
        void SetTarget(Transform target);
        void SetSpline(SplineComputer spline);
        void SetAttackPoint(Vector3 point);
        void Attack();

    }
    public interface ISpawner
    {
        void Spawn();
        void SetSpawnCount(int count);
    }


   public interface ISpawnable
    {
        void SpawnDefault();
        void Hide();
        void OnSpawn();
        void Activate(ActivatorModes mode);
        void SetSettings(object settings);
    }

    public interface IBreakable
    {
        void Break();
    }
    public interface IDestroyable
    {
        void Destroy();
    }

    public interface IWeaponTarget
    {
        bool Slash(Plane plane);
        bool KillAndPush(Vector3 force);
        bool PushAway(Vector3 origin, float force);
        Transform GetTransform();
    }

    public interface IDamagable
    {
        void TakeHit();
    }
    public interface ISclicing
    {
        Plane GetSlicePlane();
    }
}