using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Commongame;
namespace TKRunner
{
    public class DragEffectsManager : MonoBehaviour
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
        [SerializeField] private GameObject DragAuraPF;
        [SerializeField] private float auraScale;
        [Space(5)]
         private float ReferenceEffectSize  = 1;

        private DragEffect dragEffectInst;
        private DragEffect notAllowedInst;
        private Transform _CurrentTarget;

        private DragAuraManager _dragAuraManager;
        private GameObject dragAuraInst;

        private Vector3 _effectOffset = Vector3.zero;
        public void InitOffset(Vector3 offset)
        {
            _effectOffset = offset;
        }
        public void ShowEffectDrag(Transform dragPoint, Transform curvePoint)
        {
            GameManager.Instance.sounds.StartSoundEffect(Sounds.Magic);
            if (dragEffectInst == null)
                dragEffectInst = Instantiate(dragEffectPF);
            else
                dragEffectInst.gameObject.SetActive(true);
            _CurrentTarget = dragPoint;

            dragEffectInst.transform.SetParent(null);
            dragEffectInst.Init(dragPoint, SourcePoint, curvePoint, _effectOffset);
            if(dragParticles != null)
            {
                ShowingParticles();
            }
        }

        public void InstAura(Transform target, Vector3 offset, float Size, Color _color)
        {
            if (dragAuraInst == null)
                dragAuraInst = Instantiate(DragAuraPF);
            else
                dragAuraInst.SetActive(true);
            if (_dragAuraManager == null) _dragAuraManager = dragAuraInst.GetComponent<DragAuraManager>();
            dragAuraInst.transform.parent = null;

            float scale = DragAuraPF.transform.localScale.x *  Size / ReferenceEffectSize;
            _dragAuraManager.SetSize(scale);
            _dragAuraManager.SetParticlesSize(Size / ReferenceEffectSize);
            _dragAuraManager.SetColor(_color);
            dragAuraInst.transform.parent = target;
            dragAuraInst.transform.localPosition = offset;
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
            _effectOffset = Vector3.zero;
            GameManager.Instance.sounds?.StopLoopedEffect(Sounds.Magic);
            _CurrentTarget = null;
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
            while (dragParticles && dragParticles.isPlaying && _CurrentTarget != null)
            {
                dragParticles.gameObject.transform.LookAt(_CurrentTarget);
                await Task.Yield();
            }
        }


        public void ShowEffectNotAllowed(Joint target)
        {
           // await NotAllowedEffect(target);
        }


    }
}