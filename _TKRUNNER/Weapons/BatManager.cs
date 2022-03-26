using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;
using System;
namespace TKRunner
{
    public class BatManager : Weapon
    {
        [SerializeField] private float PushForce = 50;
        private Vector3 swingDir = new Vector3();
        private Action<Collider> weaponAction;
        public Vector3 GetPushForce(Transform target)
        {
            Vector3 force = ((target.position - transform.position).normalized + swingDir + Vector3.up/4) * PushForce;
            force.y = 0;
            return force;
        }
        public void RightSwing()
        {
            swingDir = transform.parent.right;
        }
        public void LeftSwing()
        {
            swingDir = -transform.parent.right;
        }
        private void HitTarget(Collider other)
        {
            IWeaponTarget target = other.GetComponent<IWeaponTarget>();
            if (target == null) return;
            if( target.KillAndPush(GetPushForce(target.GetTransform())) )
                AddHitsCount();
        }
        public override void Activate()
        {
            base.Activate();
            weaponAction = HitTarget;
        }
        public override void DeActivate()
        {
            base.DeActivate();
            weaponAction = null;
        }
        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case Tags.NormalDummy:
                    weaponAction?.Invoke(other);
                    break;
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