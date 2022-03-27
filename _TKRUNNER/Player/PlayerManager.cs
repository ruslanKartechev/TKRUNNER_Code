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
        public void TurnBackwards(bool runAnim = true, bool clockwise = true)
        {
            if (runAnim)
            {
                mAnim.SetTrigger("ForwardToBackward");
                mAnim.ResetTrigger("BackwardToForward");
            }
            if (currentDir == 'b') return;
            currentDir = 'b';
            if (TurningRoutine != null) StopCoroutine(TurningRoutine);
      
            if (clockwise == true)
            {
                TurningRoutine = StartCoroutine(Turning(180f));
            }
            else
            {
                TurningRoutine = StartCoroutine(Turning(-180f));
            }
 
        }
        public void TurnForward()
        {
            mAnim.SetTrigger("BackwardToForward");
            mAnim.ResetTrigger("ForwardToBackward");
            if (currentDir == 'f') return;
            if (TurningRoutine != null) StopCoroutine(TurningRoutine);
           
            TurningRoutine = StartCoroutine(Turning(0));
            currentDir = 'f';
        }
        private IEnumerator Turning(float end)
        {
            float elapsed = 0f;
            float start = Model.eulerAngles.y;
            if (start == 360) start = 0;
            if (end == 360) end = 0;
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
           angle =  Mathf.Clamp(angle, -180, 180);
            Model.localEulerAngles = new Vector3(Model.localEulerAngles.x,
                angle,
                Model.localEulerAngles.z);
        }

        #region Dragging

        public void OnDragStart(Vector3 position)
        {
            OnDragMove(position);
            mAnim.SetFloat("Blend", 0);
 
        }
        public void OnDragEnd(Action onAnimEnd)
        {
            onEndAction = onAnimEnd;
            mAnim.Play(AnimNames.MagicThrow, BaseAnimLayer, 0);
            GameManager.Instance._data.currentWeapon = WeaponType.Default;
            InitRotationCountdown();
            GameManager.Instance._events.WeaponEquipped.Invoke();
        }

        public void OnDragMove(Vector3 position)
        {
            double my = _controller.currentPercent;
            SplineSample res = new SplineSample();
            follower.Project(position, res);
            Vector3 distance = position - transform.position;
            float proj = Vector3.Dot(distance, transform.right);
            if (res.percent < my)
            {
                if (proj >= 0)
                    TurnBackwards(true,true);
                else
                {
                   
                    TurnBackwards(true,false);
                }
                   
            }
            else
            {
                TurnForward();
            }
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
        #endregion



        public void OnDamage()
        {
            if (TurningRoutine != null) StopCoroutine(TurningRoutine);
            if (RotationHandlerRoutine != null) StopCoroutine(RotationHandlerRoutine);
            mAnim.ResetTrigger("ForwardToBackward");
            mAnim.SetTrigger("BackwardToForward");
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

    }


}
