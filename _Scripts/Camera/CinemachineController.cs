using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using General;
public class CinemachineController : MonoBehaviour
{
   [SerializeField]  private CinemachineVirtualCamera virtualCamera;

    private Transform prevTarget;
    void Start()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        GameManager.Instance.eventManager.LevelLoaded.AddListener(OnLevelLoaded);
    }

    private void OnLevelLoaded()
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

    }
    public void ResetMachine()
    {
        virtualCamera.Follow = prevTarget;
        

    }
}
