using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using General.Events;
using General.Data;

namespace General.Cam
{
    public class CameraController : MonoBehaviour
    {
        public bool UseCameraShake = true;
        public CinemachineController _cinemachine;
        public CameraFollowManager _follower;
        public CameraShake _shaker;
        private void Start()
        {
            if (_follower == null)
                _follower = GetComponent<CameraFollowManager>();
            _follower?.Init(this);
            if (_shaker == null)
                _shaker = GetComponent<CameraShake>();
            _shaker?.SetTarget(Camera.main.transform);
            GameManager.Instance.eventManager.LevelEndreached.AddListener(OnLevelEndReached);
            if (UseCameraShake)
                GameManager.Instance.eventManager.Impact.AddListener(OnImpact);

        }
        private void OnImpact()
        {
            _shaker?.OnImpact(Random.Range(5,10));
        }
        private void OnLevelEndReached()
        {
            _cinemachine.StopMachineFollow();
            Transform EndPos = GameManager.Instance.data.currentInst.EndPos;
            CameraPositioningData data = new CameraPositioningData();
            data.Target = EndPos;
            data.MoveTime = 1.5f;
            _follower.SetPosition(data);
        }


    }
}