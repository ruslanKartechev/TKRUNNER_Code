using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
namespace General.Cam
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


        private Coroutine positioning;

        public void Init(CameraController _controller)
        {
            controller = _controller;

        }
        public void SetPosition(CameraPositioningData _data)
        {
            if (positioning != null) StopCoroutine(positioning);
            positioning = StartCoroutine(PositionSetting(_data.Target, _data.MoveTime));
            
        }

        private IEnumerator PositionSetting(Transform target, float time)
        {
            float elapsed = 0f;
            Vector3 start = transform.position;
            while(elapsed <= time)
            {
                transform.position = Vector3.Slerp(start, target.position, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = target.position;

        }




    }
}

