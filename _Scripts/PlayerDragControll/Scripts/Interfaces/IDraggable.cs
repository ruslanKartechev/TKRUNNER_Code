using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDraggable
{
    void DragStart(DragPlane dragPlane, Joint dragJoint, Action Break);
    void FlyToTarget(Transform target, float speed);
    void DragEnd();
    bool IsActive();

    bool CreateDragAura();
    Vector3 AuraOffset();

    GameObject GetGameObject();
    bool AllowDrag();
    float GetPlaneHeight();
    Rigidbody GetRigidBody();
}
