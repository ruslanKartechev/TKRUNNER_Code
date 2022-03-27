using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Events;
using Commongame.Data;
using TKRunner;
namespace Commongame.Cam
{
    public class CameraController : MonoBehaviour
    {
        public bool UseCameraShake = true;
        public CinemachineController _cinemachine;
        public CameraFollowManager _follower;
        //public CameraShake _shaker;
        private void Start()
        {
            if (_follower == null)
                _follower = GetComponent<CameraFollowManager>();
            _follower?.Init(this);
   
            GameManager.Instance._events.PlayerWin.AddListener(OnLevelEndReached);
            GameManager.Instance._events.PlayerLose.AddListener(OnPlayerLoose);
            GameManager.Instance._events.Impact.AddListener(OnImpact);
            GameManager.Instance._events.LevelLoaded.AddListener(OnNewLevel);
           
        }

        private void OnNewLevel()
        {
            _follower.StopTracking();
            _cinemachine.ResetCinemachine();
            _cinemachine.TeleportToStart();
        }
        private void OnImpact()
        {
            if (!UseCameraShake) return;
            _cinemachine.Shake();
        }
        private void OnLevelEndReached()
        {
            Transform EndPos = GameManager.Instance._data.currentInst.WinCamPos;
            MoveToPoint(EndPos);
            RotateToPoint(EndPos);
        }
        private void OnPlayerLoose()
        {

            Transform EndPos = GameManager.Instance._data.currentInst.LooseCam;
            EndPos.parent = GameManager.Instance._data.currentInst.transform;
            MoveToPoint(EndPos);
        }
        private void MoveToPoint(Transform target)
        {
            _cinemachine.StopMachineFollow();
            CameraPositioningData data = new CameraPositioningData();
            data.Target = target;
            data.MoveTime = 2.2f;
            _follower.SetPosition(data);
            _follower.SetRotation(data);
        }
        private void RotateToPoint(Transform target)
        {
            _cinemachine.StopMachineLookAt();
            CameraPositioningData data = new CameraPositioningData();
            data.Target = target;
            data.MoveTime = 2.2f;
            _follower.SetRotation(data);
        }

    }
}