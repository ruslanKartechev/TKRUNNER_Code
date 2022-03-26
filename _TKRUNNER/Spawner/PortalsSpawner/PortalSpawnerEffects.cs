using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace TKRunner
{
    public class PortalSpawnerEffects : MonoBehaviour
    {
        [SerializeField] Transform Target;   
        [SerializeField] private float FadeTime = 0.5f;
        [SerializeField] private float StartScale;
        [SerializeField] private float EndScale;

        private Coroutine _fading;
        public void Hide()
        {
            Target.gameObject.SetActive(false);

        }

        private void StopFading()
        {
            if (_fading != null) StopCoroutine(_fading);

        }
        public void FadeIn()
        {
            Target.gameObject.SetActive(true);
            StopFading();
            _fading = StartCoroutine(Fading(StartScale, EndScale,FadeTime));
        }

        public void FadeOut()
        {
            StopFading();
            _fading = StartCoroutine(Fading(EndScale, StartScale, FadeTime,OnFadeOut));
        }
        private void OnFadeOut()
        {
            Target.gameObject.SetActive(false);
        }
        private IEnumerator Fading(float start, float end, float time, Action onEnd = null)
        {
            float elapsed = 0f;

            while(elapsed <= time)
            {
                Target.localScale = Vector3.one * Mathf.Lerp(start, end, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Target.localScale = Vector3.one * end;
            onEnd?.Invoke();

        }



    }
}