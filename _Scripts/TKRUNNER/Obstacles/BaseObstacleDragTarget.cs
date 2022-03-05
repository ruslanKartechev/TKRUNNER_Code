using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKRunner
{
    public class BaseObstacleDragTarget : MonoBehaviour, IDraggable
    {
        public Collider TriggerColl;
        public Rigidbody _RB;
        public Vector3 _AuraOffset = new Vector3();
        public float _planeHeight = 1f;
        public BaseObstacle _obstacle;
        public virtual void Init(BaseObstacle obstacle)
        {
            _obstacle = obstacle;
        }

        public bool AllowDrag()
        {
            return true;
        }

        public Vector3 AuraOffset()
        {
            return _AuraOffset;
        }

        public bool CreateDragAura()
        {
            return true;
        }

        public void DragEnd()
        {
            _obstacle.OnDragEnd();
        }

        Action breakDrag;
        public void DragStart(DragPlane dragPlane, Joint dragJoint, Action Break)
        {
            _obstacle.OnDragStart();
            breakDrag = Break;
        }

        public void BreakConnection()
        {
            if (breakDrag != null)
                breakDrag.Invoke();
            breakDrag = null;
        }

        public void FlyToTarget(Transform target, float speed)
        {

        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public float GetPlaneHeight()
        {
            return _planeHeight;
        }

        public Rigidbody GetRigidBody()
        {
            return _RB;
        }

        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }


        public void SetDraggable()
        {
            TriggerColl.enabled = true;
        }

        public void SetNonDraggable()
        {
            TriggerColl.enabled = false;
        }


    }
}