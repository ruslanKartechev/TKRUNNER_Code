using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
using General;
using System;
namespace TKRunner
{
    public class BaseObstacle : MonoBehaviour
    {
        [SerializeField] protected Rigidbody RB;
        [SerializeField] protected float PushForceMultiplyer = 1f;
        [Space(5)]
        [SerializeField] protected int MaxHits;
        [Space(5)]
        [SerializeField] protected Collider MainCollider;
        [Space(5)]
        [SerializeField] private GameObject PiecesRoot;
        [SerializeField] private BaseObstacleDragTarget DragTarget;

        protected IBreakable _breakble;
        protected int Hits = 0;
        private bool CheckCollisions = false;
        private Action _Grounded;
        private void Start()
        {
            _breakble = PiecesRoot.GetComponent<IBreakable>();
            DragTarget?.Init(this);
        }

        protected void AddHitCount()
        {
            Hits++;
            if(Hits >= MaxHits)
            {
                Break();
            }
        }
        protected void Break()
        {
            DragTarget.BreakConnection();
            _breakble?.Break();
            gameObject.SetActive(false);
        }
        public void OnDragStart()
        {
            CheckCollisions = true;
        }
        public void OnDragEnd()
        {
            //CheckCollisions = false;
            _Grounded = OnGroundCollision;
        }
        protected void OnGroundCollision()
        {
            _Grounded = null;
            Break();
        }
        protected virtual void OnDummyCollision(Transform hit)
        {
            DummyManager dummy = hit.gameObject.GetComponent<DummyManager>();
            Vector3 vel = GameManager.Instance.data.CurrentDragVelocity;
            vel.y = 0;
            if (dummy == null) return;
            if ( vel.magnitude >= GameManager.Instance.data.currentInst.Data._collisionData.BoxCollisionForceThreshold)
            {
                //Debug.Log("strong enough: ");
                dummy.KillAndPush((vel ) * PushForceMultiplyer);
                AddHitCount();
            }
            //else
            //    Debug.Log("Too weak collision: " +
            //        " \nThreshold: " + GameManager.Instance.data.currentInst.Data._collisionData.BoxCollisionForceThreshold);
        }
        private void OnCollisionEnter(Collision collision)
        {

            if(CheckCollisions == true)
            {
                switch (collision.collider.tag)
                {
                    case Tags.Ground:
                        _Grounded?.Invoke();
                        break;
                    case Tags.NormalDummy:
                        OnDummyCollision(collision.transform);
                     
                        break;
                }
            }
        }







    }
}