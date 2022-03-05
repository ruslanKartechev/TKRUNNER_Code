using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using General.Data;
namespace TKRunner
{
    public class SwordManager : Weapon
    {
        [SerializeField] private float PushForce = 10;
     
        public Plane GetSlicePlane()
        {
            return new Plane(transform.up + new Vector3(0,1,0)*Random.Range(-0.2f,0.2f),transform.position);
        }



        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == Tags.NormalDummy)
            {
                DummyManager dummy = other.GetComponent<DummyManager>();
                dummy.Slash(GetSlicePlane());
                AddHitsCount();
            }
        }
    }
}