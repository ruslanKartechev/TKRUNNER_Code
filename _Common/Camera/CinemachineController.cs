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
        private CinemachineBasicMultiChannelPerlin noise;
        void Start()
        {
            if (virtualCamera == null)
                virtualCamera = GetComponent<CinemachineVirtualCamera>();
 
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }


        public void Shake()
        {
            if (noise == null) return;
            if (_shaking != null) StopCoroutine(_shaking);
            _shaking = StartCoroutine(Shaking(_shakingSettings));
            Vibration.VibratePop();
        }

        private IEnumerator Shaking(CVCShakingSettings shakeData)
        {
            noise.m_AmplitudeGain = shakeData.ShakeAmp;
            noise.m_FrequencyGain = shakeData.ShakeFreq;
            yield return new WaitForSeconds(shakeData.Duration);
            noise.m_AmplitudeGain = shakeData.NormaAmp;
            noise.m_FrequencyGain = shakeData.NormalFreq;

        }

        public void ResetCinemachine()
        {
            virtualCamera.LookAt = GameManager.Instance.data.currentInst.lookObj;
            virtualCamera.Follow = GameManager.Instance.data.currentInst.followObj;

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
        public void ResetMachine()
        {
            virtualCamera.Follow = prevTarget;
        }
    }
}