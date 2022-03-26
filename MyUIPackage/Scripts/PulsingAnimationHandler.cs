using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PulsingAnimationHandler : MonoBehaviour
{
    [SerializeField] private RectTransform Target;
    [SerializeField] private float PulsingSpeed;
    [Tooltip("Magnitude of change as a coefficient of original scale")]
    [SerializeField] private float Magnitude = 0.1f;
    [Tooltip("Return to original slowly or immidiately")]
    [SerializeField] private bool useStoppingRoutine = true;

    private Coroutine animRoutine;
    private Coroutine stoppingRoutine;
    private Vector3 originalScale = new Vector3();

    private void Start()
    {
        if (Target != null)
            originalScale = Target.localScale;
    }

    public void SetTarget(RectTransform target)
    {
        Target = target;
        originalScale = Target.localScale;
    }


    public void StartAnimation(bool restart = false)
    {
        if (gameObject.activeInHierarchy == false)
            return;

        if(animRoutine != null && restart == true)
        {
            StopAllCoroutines();
            Target.localScale = originalScale;
            animRoutine = StartCoroutine(AnimationHandler());
        }else if(animRoutine == null)
            animRoutine = StartCoroutine(AnimationHandler());

    }

    public void StopAnimation()
    {
        if (animRoutine == null || gameObject.activeInHierarchy == false)
        {
            //Target.localScale = originalScale;
            return;
        }
          
        StopCoroutine(animRoutine);
        animRoutine = null;
        if(useStoppingRoutine == false)
            Target.localScale = originalScale;
        else
        {
            stoppingRoutine = StartCoroutine(StoppingHandler());
        }

    }

    private IEnumerator AnimationHandler()
    {
        originalScale = Target.localScale;
        Vector3 maxScale = originalScale * (1+Magnitude);
        Vector3 minScale = originalScale * (1-Magnitude);
        float HalfPeriodTime = (maxScale - originalScale).magnitude / PulsingSpeed;
        float timeElapsed = 0;
        while (true)
        {

            Vector3 startScale = Target.localScale;
            timeElapsed = 0;
            while (timeElapsed < HalfPeriodTime)
            {
                timeElapsed += Time.deltaTime;
                Target.localScale = Vector3.Lerp(startScale,maxScale,timeElapsed/HalfPeriodTime);
                yield return null;
            }
            startScale = Target.localScale;
            timeElapsed = 0;
            while (timeElapsed < 2*HalfPeriodTime)
            {
                timeElapsed += Time.deltaTime;
                Target.localScale = Vector3.Lerp(startScale, minScale, timeElapsed / (2*HalfPeriodTime));
                yield return null;
            }
            startScale = Target.localScale;
            timeElapsed = 0;
            while (timeElapsed < HalfPeriodTime)
            {
                timeElapsed += Time.deltaTime;
                Target.localScale = Vector3.Lerp(startScale, originalScale, timeElapsed / HalfPeriodTime);
                yield return null;
            }
        }
    }


    private IEnumerator StoppingHandler()
    {
        Vector3 startScale = Target.localScale;
        float endTime = (originalScale - startScale).magnitude / PulsingSpeed;
        float timeElapsed = 0f;
        while(timeElapsed < endTime)
        {
            timeElapsed += Time.deltaTime;
            Target.localScale = Vector3.Lerp(startScale,originalScale,timeElapsed/endTime);
            yield return null;
        }
        Target.localScale = originalScale;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        animRoutine = null;
        stoppingRoutine = null;
    }

}
