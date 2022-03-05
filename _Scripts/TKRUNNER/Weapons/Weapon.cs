using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using System;
using General.Data;
namespace TKRunner
{
    public class Weapon : MonoBehaviour
    {
        public bool PlaySounds = true;
        public SoundNames myHitSound;
        public SoundNames mySwingSound;
        //protected string SwingName;
        // protected string HitName;
        [SerializeField] protected AudioSource swingSource;
        [SerializeField] protected AudioSource hitSource;
        [SerializeField] protected float HitsToBreak;
        public Action BreakWeapon;

        protected int hitCount = 0;
        protected void PlaySwingSound()
        {
            if(PlaySounds)
                GameManager.Instance.sounds.PlaySingleTime(mySwingSound.ToString());
        }
        protected void PlayHitSound()
        {
            if(PlaySounds)
                GameManager.Instance.sounds.PlaySingleTime(myHitSound.ToString());
        }

        protected virtual void AddHitsCount()
        {
            hitCount++;
            if (hitCount >= HitsToBreak)
            {
                BreakWeapon?.Invoke();
            }
        }

    }
}