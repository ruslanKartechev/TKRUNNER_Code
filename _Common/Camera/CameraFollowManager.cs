using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
namespace Commongame.Cam
{
    [System.Serializable]
    public class CameraPositioningData
    {
        public Transform Target;
        public float MoveTime;

    }

    public class CameraFollowManager : MonoBehaviour
    {
        private CameraController controller;
        private Coroutine _posSetting;
        private Coroutine _rotSetting;

        private Coroutine _posTracking;
        private Coroutine _rotTracking;


        public void Init(CameraController _controller)
        {
            controller = _controller;

        }
        public void SetPosition(CameraPositioningData _data)
        {
            if (_posSetting != null) StopCoroutine(_posSetting);
            _posSetting = StartCoroutine(PositionSetting(_data.Target, _data.MoveTime));
            
        }
        public void SetRotation(CameraPositioningData _data)
        {
            if (_rotSetting != null) StopCoroutine(_rotSetting);
            _rotSetting = StartCoroutine(RotationSetting(_data.Target, _data.MoveTime));
        }
        public void StopTracking()
        {
            if (_posSetting != null) StopCoroutine(_posSetting);
            if (_rotSetting != null) StopCoroutine(_rotSetting);
            if (_posTracking != null) StopCoroutine(_posTracking);
            if (_rotTracking != null) StopCoroutine(_rotTracking);

        }
        private IEnumerator PositionSetting(Transform target, float time)
        {
            float elapsed = 0f;
            Vector3 start = transform.position;
            while(elapsed <= time && target)
            {
                
                transform.position = Vector3.Slerp(start, target.position, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (target)
            {
                transform.position = target.position;
#if UNITY_EDITOR
                _posTracking = StartCoroutine(TrackPosition(target));
#endif
            }

        }

        private IEnumerator RotationSetting(Transform target, float time)
        {
            float elapsed = 0f;
            Quaternion start = transform.rotation;
            while (elapsed <= time && target)
            {
                transform.rotation = Quaternion.Slerp(start, target.rotation, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (target)
            {
                transform.rotation = target.rotation;
#if UNITY_EDITOR
                _rotTracking = StartCoroutine(TrackRotation(target));
#endif
            }

        }


        private IEnumerator TrackPosition(Transform target)
        {
            while (transform && target)
            {
                transform.position = target.position;
                yield return null;
            }
        }


        private IEnumerator TrackRotation(Transform target)
        {
            while (transform && target)
            {
                transform.rotation = target.rotation;
                yield return null;
            }
        }


        private void OnDisable()
        {
            StopTracking();
        }



    }
}

