using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using Commongame.Sound ;
using Commongame.Data;
using Commongame;
namespace TKRunner
{


    public enum RagdollStates
    {
        Passive, Active,Drag,Fly,TP
    }
    public class DummyRagdollManager : RagdollManager
    {

        [Space(5)]
        public Collider TriggerColl;

        [Space(10)]
        private DummyManager manager;
        private bool IsSliced = false;
        private CancellationTokenSource checkToken;
        private RagdollStates currentState;

        [HideInInspector] public DummyComponents _Components;


        public void TestPush()
        {
            ControllRB.isKinematic = false;
            ControllRB.AddForce(-Vector3.forward*10);
        }


        private void Start()
        {
            if (_Components == null) _Components = GetComponent<DummyComponents>();
            InitCollider(false);
            currentState = RagdollStates.Passive;


        }

        public void Init(DummyManager _dummy, DummyComponents components)
        {
            _Components = components;
            manager = _dummy;
        }

        public void InitCollider(bool state)
        {
            if (TriggerColl != null)
                TriggerColl.enabled = state;
        }

        public void PrepareSlice()
        {
            if (checkToken != null)
                checkToken.Cancel();
            IsSliced = true;
            ControllRB.gameObject.SetActive(false);
        }



        public void SetPureRagdoll()
        {
            ControllRB.gameObject.SetActive(false);
            EnableColliders();
            foreach (Rigidbody rb in allRbs)
            {
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = false;
                }
            }
        }

        public override void SetActive()
        {
            base.SetActive();
            currentState = RagdollStates.Active;
        }
        public override void SetPassive()
        {
            base.SetPassive();
            currentState = RagdollStates.Passive;
        }

        protected override void EnableColliders()
        {
            base.EnableColliders();
            InitCollider(true);
        }

        public void PushDoll(Vector3 force, bool useGravity, ForceMode mode = ForceMode.Impulse)
        {
            //while (currentState == RagdollStates.Passive)
            //{
            //    await Task.Yield();
            //}
            ControllRB.isKinematic = false;
            ControllRB.useGravity = useGravity;

            if (force.magnitude >= GameManager.Instance._data.currentInst.Data._collisionData.MaxRagdollPushForce)
            {
                force = force.normalized* GameManager.Instance._data.currentInst.Data._collisionData.MaxRagdollPushForce;
            }

            ControllRB.AddForce(force, mode);
        }
    


        public void StartGroundCheck(float time)
        {
            if (checkToken != null)
                checkToken.Cancel();
            checkToken = new CancellationTokenSource();
            DisableRoutine(checkToken.Token,time);
        }
        public async void DisableRoutine(CancellationToken token, float time)
        {
            await Task.Delay((int)(time * 1000));
            if(token.IsCancellationRequested == false && IsSliced == false)
            {

               if(MainBoneRB.velocity.magnitude > 1)
                {
                    manager.DisableDummy();
                }
            }

        }

        public void HideCollider()
        {
            InitCollider(false);
        }

        private void OnDisable()
        {
            if(checkToken != null)
                checkToken.Cancel();
        }

    }



}