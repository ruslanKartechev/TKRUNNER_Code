using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using General;
using General.Data;
namespace TKRunner
{
    public class PlayerDragEffectsManager : MonoBehaviour
    {
        [SerializeField] private Transform SourcePoint;
        [SerializeField] private Transform ParticlesPoint;
        [Space(5)]
        [SerializeField] private DragEffect dragEffectPF;
        [SerializeField] private DragEffect notAllowedPF;
        [Space(5)]
        [SerializeField] private float notAllowedEffectTime = 0.3f;
        [Space(5)]
        [SerializeField] private ParticleSystem dragParticles;
        private DragEffect dragEffectInst;
        private DragEffect notAllowedInst;
        private Joint currentJoint;
        [SerializeField] private GameObject DragAuraPF;
        [SerializeField] private float auraScale; 
        private GameObject dragAuraInst;

        public void ShowEffectDrag(Joint target)
        {
            GameManager.Instance.sounds.StartSoundEffect(Sounds.Magic);
            if (dragEffectInst == null)
                dragEffectInst = Instantiate(dragEffectPF);
            else
                dragEffectInst.gameObject.SetActive(true);
            currentJoint = target;

            dragEffectInst.transform.SetParent(null);
            dragEffectInst.Init(target, SourcePoint);
            if(dragParticles != null)
            {
                ShowingParticles();
            }
        }


        public void InstAura(Transform target, Vector3 offset)
        {
            if (dragAuraInst == null)
                dragAuraInst = Instantiate(DragAuraPF);
            else
                dragAuraInst.SetActive(true);
            dragAuraInst.transform.parent = target;
            dragAuraInst.transform.localPosition = Vector3.zero + offset;
        }

        public void HideAura()
        {
            if (dragAuraInst != null)
            {
                dragAuraInst.transform.parent = null;
                dragAuraInst.gameObject.SetActive(false);
            }
        }



        public void HideEffectDrag()
        {
            GameManager.Instance.sounds.StopLoopedEffect(Sounds.Magic);
            currentJoint = null;
            if(dragEffectInst != null)
                dragEffectInst.gameObject.SetActive(false);
            if (notAllowedInst != null)
                notAllowedInst.gameObject.SetActive(false);
            dragParticles?.Stop();
            HideAura();
        }
        private async void ShowingParticles()
        {
            dragParticles?.Play();
            while (dragParticles.isPlaying && currentJoint != null)
            {
                dragParticles.gameObject.transform.LookAt(currentJoint.transform);
                await Task.Yield();
            }
        }


        public async void ShowEffectNotAllowed(Joint target)
        {
           // await NotAllowedEffect(target);
        }
        private async Task NotAllowedEffect(Joint target)
        {
            notAllowedInst = Instantiate(notAllowedPF);
            notAllowedInst.transform.SetParent(null);
            notAllowedInst.Init(target, SourcePoint);

            await Task.Delay((int)(1000* notAllowedEffectTime));

            if (notAllowedInst != null && notAllowedInst.gameObject != null)
                notAllowedInst.gameObject.SetActive(false);
            await Task.Yield();
        }

    }
}