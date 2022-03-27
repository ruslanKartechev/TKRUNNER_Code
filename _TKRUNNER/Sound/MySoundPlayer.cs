using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Sound;

namespace Commongame
{
    public class MySoundPlayer : MonoBehaviour, ISoundEffect
    {
        public SoundNames _sound;
        //  [SerializeField] private string mySoundName;

        public void PlayEffectOnce()
        {

            GameManager.Instance._sounds.PlaySingleTime(_sound.ToString());
        }

        public void StartEffect()
        {
            GameManager.Instance._sounds.StartSoundEffect(_sound.ToString());
        }

        public void StopEffect()
        {
            GameManager.Instance._sounds.StopLoopedEffect(_sound.ToString());
        }
    }
}