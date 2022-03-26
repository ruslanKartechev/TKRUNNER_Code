using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using System;
using Commongame.Data;
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
        [Header("Particles")]
        [SerializeField] protected ParticleSystem _particleEffectPF;
         protected ParticleSystem _particlesInst;
        [HideInInspector] public Action BreakWeapon;

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
        protected virtual void SetParticles()
        {
            if (_particleEffectPF == null) return;
            _particlesInst = Instantiate(_particleEffectPF);
        }
        protected virtual void HideParticales()
        {
            if (_particlesInst == null) return;
            _particlesInst.Stop();
        }
        protected virtual void SetParticlesPos(Vector3 pos)
        {
            if (_particlesInst == null) return;
            _particlesInst.gameObject.transform.position = pos;

        }
        protected virtual void ShowParticles()
        {
            if (_particlesInst == null) return;
                _particlesInst.Play();
        }

        protected virtual void DestroyParticles()
        {
            if (_particlesInst == null) return;
            Destroy(_particlesInst);
        }
        public virtual void Activate()
        {

        }
        public virtual void DeActivate()
        {

        }

    }
}