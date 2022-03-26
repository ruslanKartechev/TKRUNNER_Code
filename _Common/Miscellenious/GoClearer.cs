using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commongame
{
    public class GoClearer : MonoBehaviour, IGoClearer
    {
        [Header("Settings")]
        [SerializeField] private float _scaleTime = 0.5f;
        [SerializeField] private float _defaultClearDelay = 1.5f;
        public float DefaultClearDelay { get { return _defaultClearDelay; } }

        public void Clear(Transform target)
        {
            target.gameObject.SetActive(false);
        }

        public void ClearWithDelay(Transform target, float time)
        {
            StartCoroutine(ScaleClearing(target, time));
        }
        

        private IEnumerator ScaleClearing(Transform target, float delay)
        {
            yield return new WaitForSeconds(delay);
            float elapsed = 0f;
            Vector3 startScale = target.localScale;
            
            while(elapsed <= _scaleTime)
            {
                target.localScale = Vector3.Slerp(startScale,Vector3.zero, elapsed/ _scaleTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Clear(target);


        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}