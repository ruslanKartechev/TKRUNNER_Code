using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;

namespace TKRunner
{
    public class RagdollManager : MonoBehaviour
    {
        [SerializeField] protected Transform RagdollRoot;
        [Header("controll Point")]
        [SerializeField] protected Rigidbody ControllRB;
        // [SerializeField] private Joint contrJoint;
        [Header("Sample Bone")]
        [SerializeField] protected Rigidbody MainBoneRB;
        [Space(10)]
        [SerializeField] protected List<Rigidbody> allRbs;
        [SerializeField] protected List<Collider> colliders;

        public virtual void SetPassive()
        {
            foreach (Collider c in colliders)
            {
                if (c != null)
                    c.enabled = false;
            }
            foreach (Rigidbody rb in allRbs)
            {
                if (rb != null)
                    rb.isKinematic = true;
            }

        }
        public virtual void SetActive()
        {
            EnableColliders();
            foreach (Rigidbody rb in allRbs)
            {
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = false;
                }
            }
            ControllRB.isKinematic = false;
            ControllRB.transform.parent = GameManager.Instance._data.currentInst.transform;
        }
        protected virtual void EnableColliders()
        {
            foreach (Collider c in colliders)
            {
                if (c != null)
                {
                    c.enabled = true;

                }
            }
        }

        public void GetRagdoll()
        {
            allRbs = new List<Rigidbody>();
            for (int i = 0; i < RagdollRoot.childCount; i++)
            {
                GetComp<Rigidbody>(RagdollRoot.GetChild(i), allRbs);
            }
            colliders = new List<Collider>();
            for (int i = 0; i < RagdollRoot.childCount; i++)
            {
                GetComp<Collider>(RagdollRoot.GetChild(i), colliders);
            }
            allRbs.TrimExcess();
            colliders.TrimExcess();
        }
        private void GetComp<T>(Transform target, List<T> store)
        {
            T obj = target.GetComponent<T>();
            if (obj != null && store.Contains(obj) == false)
            {
                store.Add(obj);
            }
            for (int i = 0; i < target.childCount; i++)
            {
                obj = target.GetChild(0).GetComponent<T>();
                if (obj != null && store.Contains(obj) == false)
                {
                    store.Add(obj);
                }
                GetComp<T>(target.GetChild(i), store);
            }
        }

    }
}