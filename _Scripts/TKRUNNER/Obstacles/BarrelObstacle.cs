using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using General.Data;
using System.Threading;
using System.Threading.Tasks;
namespace TKRunner
{
    public class BarrelObstacle : MonoBehaviour //, IDraggable
    {
        public void DragEnd()
        {
            
        }

        public void DragStart(DragPlane dragPlane, Joint dragJoint)
        {
            
        }

        private async void FallingHandler()
        {
            float delayTime = 3f;
            await Task.Delay((int)(delayTime * 1000));
            gameObject.SetActive(false);

        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        public bool AllowDrag()
        {
            return true;
        }
        public void FlyToTarget(Transform target, float speed)
        {

        }
        public float GetPlaneHeight()
        {
            return 1f;
        }
    }

}
