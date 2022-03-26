using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TKRunner;

namespace Commongame.Data
{


    public class DataManager : MonoBehaviour 
    {
        //[SerializeField] private DataGameMain main;
        //public DataGameMain MainGameData
        //{ get { return main; } }

        public bool EditorUIMode = false;

        [Header("Temp Runtime Data")]
        public LevelInstance currentInst;
        public float SlicingUpForce = 2f;
        public PlayerController Player;

        [Space(10)]
        public LayerMask PlayerMask;

        [HideInInspector]public Vector3 CurrentDragVelocity = new Vector3();
        [Space(10)]
        public WeaponType currentWeapon;


    }
}