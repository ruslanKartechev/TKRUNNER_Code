using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Commongame.Data;

namespace TKRunner
{

    public interface IBullet
    {
        void SetPosition(Vector3 pos);
        void SetLookDir(Vector3 dir);
        void StartBullet(Vector3 vel);
        Vector3 GetVelocity();

    }
    public class Bullet : MonoBehaviour, IBullet
    {
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected Collider coll;
        protected Action OnBulletHit = null;

        public void SetParent(Transform parent) => transform.parent = parent;
        public virtual void SetPosition(Vector3 pos)
        {
            transform.position = pos;

        }
        public virtual void StartBullet(Vector3 vel)
        {
            SetLookDir(vel);
            rb.velocity = vel;
         //   coll.isTrigger = false;

        }
        public Vector3 GetVelocity()
        {
            if(rb == null) return Vector3.zero;
            return rb.velocity;
        }
        public virtual void StartBulletForward(float speed)
        {
            rb.velocity = transform.forward * speed;
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

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case Tags.Obstacle:
                    other.gameObject.GetComponent<IDestroyable>()?.Destroy();
                    break;
                case Tags.Box:
                    other.gameObject.GetComponent<IDestroyable>()?.Destroy();
                    break;
            }
        }




    }
}