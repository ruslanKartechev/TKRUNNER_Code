using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Bullet : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider coll;
    protected Action OnBulletHit = null;
    public virtual void SetPosition(Vector3 pos)
    {
        transform.position = pos;

    }
    public virtual void StartBullet(Vector3 vel)
    {
        SetLookDir(vel);
        rb.velocity = vel;
        coll.isTrigger = false;

    }
    public virtual void StartBulletForward(float speed)
    {
        rb.velocity = transform.forward * speed;
        coll.isTrigger = false;
    }
    public virtual void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }
    public virtual void SetLookDir(Vector3 dir)
    {
        transform.LookAt(transform.position + dir);
    }
    public virtual void SetBulletHitAction(Action onHit)
    {
        OnBulletHit = onHit;
    }
    public virtual void ShowEffect()
    {

    }
    

}
