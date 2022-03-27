using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Commongame;


namespace TKRunner {

    [System.Serializable]
    public class CVCShakingSettings
    {
        public float NormaAmp;
        public float NormalFreq;
        public float ShakeAmp;
        public float ShakeFreq;
        public float Duration;
    }

    public class CinemachineController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [Header("Shaking Params")]
        [SerializeField] private CVCShakingSettings _shakingSettings;
        private Transform prevTarget;

        private Coroutine _shaking;
        private CinemachineBasicMultiChannelPerlin cam_noise;
        private Cinemachine3rdPersonFollow cam_follow;
        private CinemachineComposer cam_aim;


        void Start()
        {
            if (virtualCamera == null)
                virtualCamera = GetComponent<CinemachineVirtualCamera>();
 
            cam_noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cam_follow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            cam_aim = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        }


        public void Shake()
        {
            if (cam_noise == null) return;
            if (_shaking != null) StopCoroutine(_shaking);
            _shaking = StartCoroutine(Shaking(_shakingSettings));
            Vibration.VibratePop();
        }

        private IEnumerator Shaking(CVCShakingSettings shakeData)
        {
            cam_noise.m_AmplitudeGain = shakeData.ShakeAmp;
            cam_noise.m_FrequencyGain = shakeData.ShakeFreq;
            yield return new WaitForSeconds(shakeData.Duration);
            cam_noise.m_AmplitudeGain = shakeData.NormaAmp;
            cam_noise.m_FrequencyGain = shakeData.NormalFreq;

        }


        public void TeleportToStart()
        {
            StopMachineFollow();
            StopMachineLookAt();
            Vector3 position = cam_follow.ShoulderOffset + GameManager.Instance._data.currentInst.followObj.position;
            virtualCamera.transform.position = position;
            virtualCamera.transform.rotation = Quaternion.LookRotation(GameManager.Instance._data.currentInst.lookObj.position);
            ResetCinemachine();
            //Quaternion rotation = Quaternion.LookRotation(position);

        }

        public void ResetCinemachine()
        {
            virtualCamera.LookAt = GameManager.Instance._data.currentInst.lookObj;
            virtualCamera.Follow = GameManager.Instance._data.currentInst.followObj;

        }

        public void StopMachineFollow()
        {
            prevTarget = virtualCamera.Follow;
            virtualCamera.Follow = null;

        }
        public void StopMachineLookAt()
        {
            virtualCamera.LookAt = null;
        }
    }
}