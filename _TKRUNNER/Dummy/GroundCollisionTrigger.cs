using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TKRunner
{
    public class GroundCollisionTrigger : MonoBehaviour
    {
        public DummyComponents Components;
        
        private void OnTriggerEnter(Collider other)
        {
       
            if(other.tag == Tags.Ground)
            {
                Components._manager.OnGroundFall?.Invoke();
            }
          
        }


    }
}