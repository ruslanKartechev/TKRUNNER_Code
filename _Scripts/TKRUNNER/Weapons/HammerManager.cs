using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
using General;
namespace TKRunner
{
    public class HammerManager : Weapon
    {
        [SerializeField] Transform HammerHead;
        [SerializeField] private float Rad;
        [SerializeField] private float Force;
        [SerializeField] private LayerMask dummyMask;

        public void OnAnimStrikeEvent()
        {
            PlayHitSound();
            GameManager.Instance.eventManager.Impact.Invoke();
            Collider[] colls = Physics.OverlapSphere(HammerHead.position, Rad, dummyMask);
    
            if (Physics.Raycast(HammerHead.position, -HammerHead.up, out RaycastHit hit, 10f))
            {
                if (hit.collider.tag == Tags.Truck)
                {
                    hit.collider.transform.GetComponent<TruckManager>()?.TakeHit();
                    return;
                }
  
            }
            foreach (Collider coll in colls)
            {
                if(coll.tag == Tags.NormalDummy || coll.tag == Tags.GuardedDummy)
                {
                    coll.gameObject.GetComponent<IWeaponTarget>()?.PushAway(HammerHead.position,Force);
                    
                }
               
            }
            AddHitsCount();

        }

    }
}