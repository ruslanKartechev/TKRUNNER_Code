using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TKRunner
{
    public class ConfettiManager : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _confetties = new List<ParticleSystem>();

        public void StartConfetti()
        {
            
            foreach(ParticleSystem particles in _confetties)
            {
                particles.gameObject.SetActive(true);
                particles.Play();
            }
        }
        public void StopConfetti()
        {
            foreach (ParticleSystem particles in _confetties)
            {
                particles.Stop();
                particles.gameObject.SetActive(false);

            }
        }


    }
}