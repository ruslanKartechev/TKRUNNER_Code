using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General.Data {
    [CreateAssetMenu(fileName = "DataGameMain", menuName = "ScriptableObjects/DataGameMain", order = 1)]
    public class DataGameMain : ScriptableObject
    {

        public MovementData moveData;
        public DraggingData dragData;
        public CollisionsData _collisionData;
        [HideInInspector] public EffectsData effects;

        //public SpawningData spawningData;

        //public AutoAimData autoAimData;




    }
}