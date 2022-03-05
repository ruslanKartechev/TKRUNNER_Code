using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using General.Data;
namespace TKRunner
{
    public class BatManager : Weapon
    {
        [SerializeField] private float PushForce = 10;
        private Vector3 swingDir = new Vector3();

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
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == Tags.NormalDummy)
            {
                IWeaponTarget target = other.GetComponent<IWeaponTarget>();
                target.KillAndPush(GetPushForce(target.GetTransform()));
                AddHitsCount();
            }
        }
    }
}