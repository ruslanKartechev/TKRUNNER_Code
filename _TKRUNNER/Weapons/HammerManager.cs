using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
namespace TKRunner
{
    public class HammerManager : Weapon
    {
        [SerializeField] Transform HammerHead;
        [SerializeField] private LayerMask castMask;
        private void Start()
        {
            SetParticles();
        }
        public override void Activate()
        {
            base.Activate();
            castMask = GameManager.Instance.data.currentInst.Data._weaponsData.HammerCastMask;
        }

        public void OnAnimStrikeEvent()
        {
            Strike();

        }

        private void Strike()
        {
            PlayHitSound();
            GameManager.Instance.eventManager.Impact.Invoke();
            SetParticlesPos(HammerHead.position);
            ShowParticles();

            Collider[] colls = Physics.OverlapSphere(HammerHead.position, 
                GameManager.Instance.data.currentInst.Data._weaponsData.HammerHitRadius, castMask);

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
                coll.GetComponent<IDestroyable>()?.Destroy();
                coll.gameObject.GetComponent<IWeaponTarget>()?.PushAway(HammerHead.position, 
                    GameManager.Instance.data.currentInst.Data._weaponsData.HammerPushForce);

            }
            AddHitsCount();
        }

    }
}