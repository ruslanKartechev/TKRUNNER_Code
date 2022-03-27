using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TKRunner;
namespace Commongame.Data {
    [CreateAssetMenu(fileName = "DataGameMain", menuName = "ScriptableObjects/DataGameMain", order = 1)]
    public class DataGameMain : ScriptableObject
    {

        public MovementData moveData;
        public DraggingData dragData;
        public CollisionsData _collisionData;
        public WeaponsData _weaponsData;
        public PlayerHealthData _PlayerHealth;

        [HideInInspector] public EffectsData effects;
        [Header("HandSettings, Not for build")]
        [Space(10)]
        public float HandMaxSpeed;


    }
}