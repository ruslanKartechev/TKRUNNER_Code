using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace TKRunner
{
    public class LazerGatesManager : MonoBehaviour
    {

        private LazerBeam beam;
        private CancellationTokenSource moveToken;
        private LazerGatesController boss;

        public void Init(LazerGatesController _boss, LazerBeam _bream)
        {
            beam = _bream;
            boss = _boss;
            beam.Init();
            SetBeamOffset();
        }

        public void StartLazer()
        {
            beam.Activate();
        }
        public void StopLazer()
        {
            beam.DeActivate();
        }
        public void StartMovement()
        {
            StopMovement();
            MovingHandler();
        }
        public void StopMovement()
        {
            moveToken?.Cancel();
            moveToken = new CancellationTokenSource();
        }
        private void SetBeamOffset()
        {
            float y = UnityEngine.Random.Range(-boss.Data.BottomMargin, boss.Data.TopMargin);
            SetBeamHeight(y);
        }

        private async void MovingHandler()
        {
            while(moveToken.IsCancellationRequested == false)
            {
                await MoveOneSIde(boss.Data.TopMargin, boss.Data.MoveSpeed);
                await MoveOneSIde(boss.Data.BottomMargin, boss.Data.MoveSpeed);
            }

        }
        private async Task MoveOneSIde(float targetY, float speed)
        {
            if(moveToken.IsCancellationRequested == false)
            {
                float elapsed = 0f;
                float startY = beam.transform.localPosition.y;
                float time = Mathf.Abs(targetY - startY) / speed;
                if(time == 0)
                {
                    time = 0.2f;
                }
                while (elapsed <= time && moveToken.IsCancellationRequested == false)
                {
                    float y = Mathf.Lerp(startY, targetY, elapsed / time);
                    SetBeamHeight(y);
                    elapsed += Time.deltaTime;
                    await Task.Yield();
                }
                if (moveToken.IsCancellationRequested == false)
                    SetBeamHeight(targetY);
            }

        }
        private void SetBeamHeight(float y)
        {
            beam.gameObject.transform.localPosition = new Vector3(
                beam.gameObject.transform.localPosition.x,
                y,
                beam.gameObject.transform.localPosition.z
                );
        }

        private void OnDisable()
        {
            moveToken?.Cancel();
        }

    }
}