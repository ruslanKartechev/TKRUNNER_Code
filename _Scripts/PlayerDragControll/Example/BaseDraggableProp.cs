using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDraggableProp : MonoBehaviour //, IDraggable
{
    public void DragStart(DragPlane dragPlane, Joint dragJoint)
    {
        
    }

    public void DragEnd()
    {

    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool IsActive()
    {
        return true;
    }

    public bool UseAutoAim()
    {
        return false;
    }
    public bool IsGuarded()
    {
        return false;
    }

    public void FlyToTarget(Transform target)
    {

    }
}
