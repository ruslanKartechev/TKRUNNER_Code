using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
namespace TKRunner
{
    public class AxeManager : Weapon, ISclicing
    {

        public override void Activate()
        {
            base.Activate();
        }

        public Plane GetSlicePlane()
        {
            return new Plane(transform.parent.up, transform.position);

        }
        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case Tags.NormalDummy:
                    IWeaponTarget dummy = other.GetComponent<IWeaponTarget>();
                    if(dummy.Slash(GetSlicePlane()))
                        AddHitsCount();
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