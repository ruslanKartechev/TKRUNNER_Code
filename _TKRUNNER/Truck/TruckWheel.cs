using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKRunner
{
    public class TruckWheel : MonoBehaviour
    {
        public Rigidbody rb;
        public Collider coll;
        public void Init()
        {
            rb.isKinematic = true;
            coll.enabled = false;
        }
        public void PushAway(Vector3 force)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
            coll.isTrigger = false;
            coll.enabled = true;
            transform.parent = null;
        }

    }
}