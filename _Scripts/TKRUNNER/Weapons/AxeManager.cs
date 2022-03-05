using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;

namespace TKRunner
{
    public class AxeManager : Weapon, ISclicing
    {



        public Plane GetSlicePlane()
        {
            return new Plane(transform.parent.up, transform.position);

        }
        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.collider.tag)
            {

                case Tags.NormalDummy:
                    collision.collider.gameObject.GetComponent<DummyManager>().Slice(GetSlicePlane());
                    break;
            }   
        }
   


    }
}