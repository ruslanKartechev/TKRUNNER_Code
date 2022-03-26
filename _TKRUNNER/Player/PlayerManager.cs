using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;
using System;
using Dreamteck.Splines;
namespace TKRunner
{
    public class PlayerManager : RunnerManager
    {
        [Header("Settings")]
        public PlayerSettings _settings;
        [Space(5)]
        public Transform Model;
        public Collider mainColl;


        private PlayerController _controller;
        private Action onEndAction = null;
        private Coroutine TurningRoutine;
        private Coroutine RotationHandlerRoutine;
        private char currentDir = 'b';


        public float CurrentPercent { get { return (float)follower.result.percent; } }

        public void Init(PlayerController controller)
        {
            OnRunStarted = PlayForwardRunAnim;
            _controller = controller;
        }

        public override void InitActive(SplineComputer spline)
        {
            base.InitActive(spline);
            mainSpeed = _settings.StartSpeed;
            mAnim.Play(AnimNames.ShowOff, 0, 0);
        }
        public float GetSpeed()
        {
            return follower.followSpeed;
        }

        private void InitRotationCountdown()
        {
            StopRotationCountdown();
            RotationHandlerRoutine = StartCoroutine(RotationCountdowm());
        }
        public void StopRotationCountdown()
        {
            if (RotationHandlerRoutine != null)
                StopCoroutine(RotationHandlerRoutine);
        }

        private IEnumerator RotationCountdowm()
        {
            float elapsed = 0f;
            while (elapsed < _settings.ForwardTurnDelay)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            TurnForward();

        }
        public void TurnBackwards(bool runAnim = true)
        {
            if (TurningRoutine != null) StopCoroutine(TurningRoutine);
            if (runAnim)
            {
                mAnim.SetTrigger("ForwardToBackward");
                mAnim.ResetTrigger("BackwardToForward");
            }
            TurningRoutine = StartCoroutine(Turning(180));
            currentDir = 'b';
        }
        public void TurnForward()
        {
            if (TurningRoutine != null) StopCoroutine(TurningRoutine);
            mAnim.SetTrigger("BackwardToForward");
            mAnim.ResetTrigger("ForwardToBackward");
            TurningRoutine = StartCoroutine(Turning(0));
            currentDir = 'f';
        }

        private IEnumerator Turning(float end)
        {
            float elapsed = 0f;
            float start = Model.eulerAngles.y;
            while (elapsed <= _settings.TurnTime)
            {
                float y = Mathf.Lerp(start, end, elapsed / _settings.TurnTime);
                SetModelAngle(y);
                elapsed += Time.deltaTime;
                yield return null;
            }
            SetModelAngle(end);
        }
        private void PlayForwardRunAnim()
        {
            mAnim.Play(AnimNames.ForwardRun);
        }
        private void SetModelAngle(float angle)
        {
            Model.localEulerAngles = new Vector3(Model.localEulerAngles.x,
                angle,
                Model.localEulerAngles.z);
        }


        public void OnDragStart()
        {
            mAnim.SetFloat("Blend", 0);
            mAnim.Play(AnimNames.MagicIdle, BaseAnimLayer,0);
            StopRotationCountdown();
            if (currentDir == 'f')
                TurnBackwards();
        }
        public void OnDragEnd(Action onAnimEnd)
        {
            onEndAction = onAnimEnd;
            mAnim.Play(AnimNames.MagicThrow, BaseAnimLayer, 0);
            GameManager.Instance.data.currentWeapon = WeaponType.Default;
            InitRotationCountdown();
            GameManager.Instance.eventManager.WeaponEquipped.Invoke();
        }
        public void OnDragBroken()
        {
            mAnim.Play(AnimNames.DummyRun);
            InitRotationCountdown();
        }
        public void OnThrowEvent()
        {
            if(onEndAction != null)
            {
                onEndAction.Invoke();
                onEndAction = null;
            }
        }

        public void OnDamage()
        {
            if (TurningRoutine != null) StopCoroutine(TurningRoutine);
            if (RotationHandlerRoutine != null) StopCoroutine(RotationHandlerRoutine);
            mAnim.SetTrigger("BackwardToForward");
            mAnim.ResetTrigger("ForwardToBackward");
            if(currentDir == 'b')
                TurnForward();
            mAnim.SetBool("DoRoll", true);
        }
        public void OnRollComplete()
        {
            mAnim.SetBool("DoRoll",false);
            _controller.Restore();

        }
        public void OnDeath()
        {
            StopAllCoroutines();
            Destroy(rb);
            mainColl.enabled = false;
            StopMoving();
            follower.enabled = false;
            mAnim.StopPlayback();
            mAnim.enabled = false;
        }


        //public void OnCollisionEnter(Collision collision)
        //{
        //    switch (collision.collider.tag)
        //    {
        //        case Tags.LevelEnd:
        //            GameManager.Instance.eventManager.LevelEndreached.Invoke();
        //            GameManager.Instance.eventManager.PlayerWin.Invoke();
        //            break;
        //    }
        //}

    }


}
