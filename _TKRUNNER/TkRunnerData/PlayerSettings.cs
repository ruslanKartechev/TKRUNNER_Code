using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TKRunner
{
    [CreateAssetMenu(fileName = "PlayerSettings_", menuName = "ScriptableObjects/PlayerSettings", order = 1)]
    public class PlayerSettings : ScriptableObject
    {
        public float StartSpeed;
        public float TurnTime = 0.5f;
        public float ForwardTurnDelay = 1f;

    }
}