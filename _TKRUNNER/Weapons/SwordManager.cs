using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Commongame.Data;
namespace TKRunner
{
    public class SwordManager : Weapon
    {
        [SerializeField] private float PushForce = 10;
        [SerializeField] private Vector3 SlicePlaneNormal = new Vector3();

        private Action<Transform> Attack;
        public Plane GetSlicePlane()
        {
            return new Plane(transform.parent.up, transform.position);

        }

        public override void Activate()
        {
            Attack = CutTarget;
        }
        public override void DeActivate()
        {
            Attack = null;
        }
        private void CutTarget(Transform target)
        {
            IWeaponTarget dummy = target.GetComponent<IWeaponTarget>();
            if( dummy.Slash(GetSlicePlane()) )
                AddHitsCount();
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case Tags.NormalDummy:
                    Attack?.Invoke(other.transform);
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