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
        public RagdollStates CurrentState { get { return currentState; } }


        public void TestPush()
        {
            //ControllRB.useGravity = true;
            ControllRB.isKinematic = false;
            ControllRB.AddForce(-Vector3.forward*10);
        }


        private void Start()
        {
            InitCollider(false);
            currentState = RagdollStates.Passive;


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
          //  Destroy(this);
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
    

        public void Init(DummyManager _dummy)
        {
            manager = _dummy;
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




        public void SetRagdollState(RagdollStates state)
        {
            currentState = state;
            switch (state)
            {
                //case RagdollStates.Drag:
                //    GameManager.Instance.eventManager.StrongDummyImpact.AddListener(OnStrongCollision);
                //    break;
                //case RagdollStates.Active:
                //    GameManager.Instance.eventManager.StrongDummyImpact.RemoveListener(OnStrongCollision);
                //    break;
                //case RagdollStates.Fly:
                //    GameManager.Instance.eventManager.StrongDummyImpact.RemoveListener(OnStrongCollision);
                //    break;
            }
        }
        public void HideCollider()
        {
            InitCollider(false);
        }
        public void OnPortalEnter(DummyPortal portal)
        {
            if (portal == null )
                return;
            PortalData data = portal.GetOutPortalData();
            if (data == null)
                return;
            InitCollider(false);
            portal.ShowEffect();
           // manager.transform.parent = null;
            manager.DragTarget.BreakConnection();
            currentState = RagdollStates.TP;
            StartCoroutine(PortalTransition(data));
        }
        private IEnumerator PortalTransition(PortalData data)
        {

            manager.Renderer.enabled = false;
            yield return null;
            manager.Renderer.enabled = true;
            //ControllJointRB.velocity = Vector3.zero;
            //yield return new WaitForSeconds(data.TPdelay);
            //contrJoint.connectedBody = null;
            //MainBoneRB.transform.position = data.outPosition; ////// NOT MAIN BONE
            //contrJoint.transform.position = data.outPosition;
            //yield return new WaitForFixedUpdate();
            //contrJoint.connectedBody = MainBoneRB;
            ////Debug.Log("OutPOs: " + data.outPosition);
            ////Debug.Log("My pos: " + ControllJointRB.position);
            //yield return null;
            //PushDoll(data.outForward ,true);
            //manager.Renderer.enabled = true;

        }

        private void OnWallCollision()
        {
            InitCollider(false);
            GameManager.Instance._sounds.PlaySingleTime(Sounds.WallHit);
            if (CurrentState == RagdollStates.Drag)
            {
                manager.DragTarget.BreakConnection();
            }


        }
        private void OnStrongCollision()
        {
            if (currentState == RagdollStates.Drag)
            {
                manager.DragTarget.BreakConnection();
                GameManager.Instance._sounds.PlaySingleTime(Sounds.DummyCollision);
            }
        }




        private void OnDisable()
        {
            if(checkToken != null)
                checkToken.Cancel();
        }

    }



}