using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TKRunner
{
    public class PlayerIKManager : MonoBehaviour
    {
        //[SerializeField] private TwoBoneIKConstraint leftHand;
        //[SerializeField] private TwoBoneIKConstraint RightHand;
        [SerializeField] private Rig rig;
        [SerializeField] private PlayerController controll;
        [SerializeField] private Transform movable;
        private Coroutine weightChanging;

        public void Init(PlayerController _controll)
        {
            controll = _controll;

        }
        public void StartHandIK()
        {
            if (weightChanging != null) StopCoroutine(weightChanging);
            weightChanging = StartCoroutine(WeightChange(1, 0.3f));
        }
        public void StopHandIK()
        {
            if (gameObject == null || gameObject.activeInHierarchy == false)
                return;
            movable.transform.position = transform.position - transform.forward + 1.5f * transform.up;
            if (weightChanging != null) StopCoroutine(weightChanging);
            weightChanging = StartCoroutine(WeightChange(0,0.3f));
        }
        public void SetWeight(float val)
        {
            rig.weight = val;
        }
        private IEnumerator WeightChange(float target, float time)
        {
            float elapsed = 0;
            float start = rig.weight;
            while (elapsed <= time)
            {
                rig.weight = Mathf.Lerp(start, target,elapsed/time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rig.weight = target;
        }

        public void Move(Vector3 newPos)
        {
            Vector3 distance = newPos - transform.position;
            movable.transform.position = newPos;
        }
        private void OnDisable()
        {
            if (weightChanging != null) StopCoroutine(weightChanging);
        }
    }
}