using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Commongame.Data
{
    using Commongame.Sound;
    [CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/Sounds", order = 1)]
    public class SoundData : ScriptableObject
    {
        public List<Sound> soundEffects = new List<Sound>();
        public List<Sound> music = new List<Sound>();
    }

}