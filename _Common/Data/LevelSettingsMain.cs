using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commongame.Data
{
    [CreateAssetMenu(fileName = "LevelSettings_", menuName = "ScriptableObjects/LevelSettings", order = 1)]
    public class LevelSettingsMain : ScriptableObject
    {
        public SpawningData spawningData;
    }
}