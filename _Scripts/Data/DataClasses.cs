using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
namespace General.Data
{

    [System.Serializable]
    public class EnemySettings
    {
        public float StartSpeed = 10f;
        public float ApproachDistance = 3f;
        public float SideSpeed = 1f;
        public float TPdistance = 10f;
        public float AttackPushForce = 10f;

    }

    [System.Serializable]
    public class CollisionsData
    {
        public float BoxCollisionForceThreshold = 5f;
        public float MaxRagdollPushForce = 25f;
    }

    [System.Serializable]
    public class DraggingData
    {
        public float DragSensitivity;
        [Space(5)]
        public float VelocityThreshold = 10;
    }


    [System.Serializable]
    public class MovementData
    {
        public float GlobalSpeedMod = 1 ;
        public float GlobalSideSpeedMod = 1f;
        [Space(5)]
        public float DummyJumpAttackDistance = 0.5f; 
    //    public float RunAnimSpeedMultiplyer;
    }

    [System.Serializable]
    public class LevelData
    {
        public GameObject lvlPF;
    }

   [System.Serializable]
   public class CylinderMeshData
   {
        public float Radius;
        public float Rad_bottom;
        public float Rad_top;
        public bool Equal = true;
        public float Thickness;
        public int SidesCount;
        public Material cylinderMaterial;
        public CylinderMeshData(CylinderMeshData from)
        {
            Radius = from.Radius;
            Rad_bottom = from.Rad_bottom;
            Rad_top = from.Rad_top;
            Equal = from.Equal;
            Thickness = from.Thickness;
            SidesCount = from.SidesCount;
            cylinderMaterial = from.cylinderMaterial;
        }
   }

    [System.Serializable]
    public class SpawningData
    {
        public int number_min;
        public int number_max;
        [Header("Half width of the track")]
        public float trackHalfWidth;
        public float DeadZone = 1f;
        //[Header("Length on the interval, where dummies spawn in track percent")]
        //public float spawnPercentLength;
        [Header("Shares of different dummy types")]
        public float NormalThreshold;
        public float GuardedThreshold;


        [Header("Vertical offset")]
        [Header("StartSettings")]
        public float Yoffset = 1f;
        [Header("Start runners Settings")]
        public bool SpawnDummiesOnStart = false;
        public int StartNum = 4;
        [Tooltip("Dummies Spawned in one row. Has to be even")]
        public int NumberPerRow = 4;
        public float ColomnSpacing = 1f;
        [Tooltip("Measerud in percent of spline")]
        public float RowSpacing = 0.1f;
        [Header("Initial speed up")]
        public float StartDuration = 1f;
        public float FromBehindDuration = 1f;
        public float AccelerationTime = 0.3f;
        [Header("Max speed on acceleration as a multiple of normal speed")]
        public float StartMaxSpeed = 1.25f;
        public float FromBehidMaxSpeed = 2f;
    }

    [System.Serializable]
    public class EffectsData
    {
        [Header("Dummy thrown effects")]
        public float freeFallTime = 2f;
        public float fallingMaxSpeed = 10f;
        //public float rollTime = 1f;
        [Space(5)]
        public float layPosTime = 1f;
        public float standUpTime = 1f;
    }


    [System.Serializable]
    public class AutoAimData
    {
        [Tooltip("Measured in pixels")]
        public int AutoAimError = 200;
    }

    [System.Serializable]
    public class DummyTruckData
    {
        public float DeployTime;
        public float JumpDistance;
        
    }

    [System.Serializable]
    public class TruckMovementData
    {
        [Header("-1 left, 0 center, 1 right")]
        public int movePattern;
        public float MainSpeed;
        public float BackwardSpeed;
        public float startOffset = 0;
        public float rightOffset = 1.2f;
        public float leftOffset = -1.2f;
        [Space(10)]           
        public float DeploymentTime;
        public float SlowTime;
        public float HideTime;
        public float TurningDistance;
        public float StopDistance;


    }
    [System.Serializable]
    public class LazerGatesData
    {
        public float TopMargin;
        public float BottomMargin;
        public float MoveSpeed;

    }

    [System.Serializable]
    public class LazerSpinnerData
    {
        public float RotSpeed;
    }

    [System.Serializable]
    public class PortalData
    {
        [HideInInspector] public Vector3 inPosition;
        [HideInInspector] public Vector3 outPosition;
        [HideInInspector] public Vector3 outForward;
        public float SucitonTime = 0.3f;
        public float TPdelay = 0f;
        public float OutPushForce = 5f;
    }

}