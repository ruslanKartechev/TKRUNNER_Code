using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace General.Sound
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        [Range(0f,1f)]
        public float volume;
        [Range(0f, 1f)]
        public float pitch;
        public AudioClip clip;
        // public AudioSource defaultSource;
        public float loopedTime_start;
        public float loopedTime_end;
    }
}