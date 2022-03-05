using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TKRunner;
namespace General.Data
{


    public class DataManager : MonoBehaviour 
    {
        //[SerializeField] private DataGameMain main;
        //public DataGameMain MainGameData
        //{ get { return main; } }

        public bool EditorUIMode = false;

        [Header("Temp Runtime Data")]
        public LevelInstance currentInst;
        public PlayerController Player;
        public Vector3 CurrentDragVelocity = new Vector3();



    }
}