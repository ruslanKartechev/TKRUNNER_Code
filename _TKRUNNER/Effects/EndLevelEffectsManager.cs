using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace TKRunner
{

    public class EndLevelEffectsManager : MonoBehaviour
    {
        [SerializeField] private float ClosingDelay = 2f;
        [Space(10)]
        [SerializeField] LanternsManager _lanternsManager;
        [SerializeField] ConfettiManager _confettiManager;

        [SerializeField] private Transform openTrigger;
        [SerializeField] private Transform closeTrigger;
        [SerializeField] private Transform MoreEffects;

        private ITriggerDetector open;
        private ITriggerDetector close;
        private ITwoStepEffect twoStep;
        private void Start()
        {
            if(openTrigger != null)
            {
                open = openTrigger.gameObject.GetComponent<ITriggerDetector>();
                open.Init(OnOpen, Tags.Player);
            }
       
            if(closeTrigger != null)
            {
                close = closeTrigger.gameObject.GetComponent<ITriggerDetector>();
                close.Init(OnClose, Tags.Player);
            }

            if(MoreEffects != null)
            {
                twoStep = MoreEffects.gameObject.GetComponent<ITwoStepEffect>();
            }
            GameManager.Instance.eventManager.LevelLoaded.AddListener(OnLevelLoaded);
     
        }

        private void OnLevelLoaded()
        {
            _lanternsManager?.StopLanterns();
            _confettiManager.StopConfetti();
        }

        public void OnOpen()
        {
            twoStep?.OnOpen();
        }
        public void OnClose()
        {
            StartCoroutine(FinishShowHandler());
        }

        private IEnumerator FinishShowHandler()
        {
            _lanternsManager?.StartLanters();
            _confettiManager?.StartConfetti();
            GameManager.Instance.eventManager.LevelEndreached.Invoke();
            GameManager.Instance.eventManager.PlayerWin.Invoke();
            yield return new WaitForSeconds(ClosingDelay);
            twoStep?.OnClose();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}