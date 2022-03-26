using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggableTarget
{
    void SendDamage(Vector3 force);
    bool IsActive();
    GameObject GetGameObject();
}
