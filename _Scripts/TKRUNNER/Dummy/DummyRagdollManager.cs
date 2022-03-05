using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UnityEditor;
using General.Data;
using General;
namespace TKRunner
{



    public enum RagdollStates
    {
        Passive, Active,Drag,Fly,TP
    }
    public class DummyRagdollManager : MonoBehaviour
    {
        [SerializeField] private Transform RagdollRoot;
        [Header("controll Point")]
        [SerializeField] private Rigidbody ControllRB;
       // [SerializeField] private Joint contrJoint;
        [Header("Sample Bone")]
        [SerializeField] private Rigidbody MainBoneRB;
        [Space(5)]
        public Collider TriggerColl;
        [Space(10)]
        [SerializeField] private List<Rigidbody> allRbs;
        [SerializeField] private List<Collider> colliders;
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

        private void InitCollider(bool state)
        {
            if (TriggerColl != null)
                TriggerColl.enabled = state;
        }

        public void SetSliced()
        {
            if (checkToken != null)
                checkToken.Cancel();
            IsSliced = true;
            foreach (Rigidbody rb in allRbs)
            {
                if (rb != null)
                {
                    CharacterJoint joint = rb.gameObject.GetComponent<CharacterJoint>();
                    if(joint != null)
                    {
                     //   joint.connectedBody = null;
                   //     Destroy(joint);
                    }
                    rb.velocity = Vector3.zero;
                    Collider c = rb.gameObject.GetComponent<Collider>();
                    if (c != null)
                        Destroy(c);
                   // rb.isKinematic = true;
                }
            }
            Destroy(this);
        }


        public void SetPassive()
        {
            foreach (Collider c in colliders)
            {
                if(c!=null)
                    c.enabled = false;
            }
            foreach (Rigidbody rb in allRbs)
            {
                if(rb != null)
                    rb.isKinematic = true;
            }
            currentState = RagdollStates.Passive;
        }
        public async Task SetActive()
        {
            EnableColliders();
            await Task.Yield();

            foreach (Rigidbody rb in allRbs)
            {
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = false;
                }
            }
            await Task.Yield();
           // await Task.Delay((int)(1000 * 1f));
            transform.parent = null;
            currentState = RagdollStates.Active;
        }


        private void EnableColliders()
        {
            InitCollider(true);
            foreach (Collider c in colliders)
            {
                if (c != null)
                {
                    c.material = null; // replace !!
                    c.enabled = true;

                }
            }
        }

        public async void PushDoll(Vector3 force, bool useGravity, ForceMode mode = ForceMode.Impulse)
        {
            while (currentState == RagdollStates.Passive)
            {
                await Task.Yield();
            }
            ControllRB.isKinematic = false;
            ControllRB.useGravity = useGravity;

            if (force.magnitude >= GameManager.Instance.data.currentInst.Data._collisionData.MaxRagdollPushForce)
            {
                force = force.normalized* GameManager.Instance.data.currentInst.Data._collisionData.MaxRagdollPushForce;
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


        public void GetRagdoll()
        {
            allRbs = new List<Rigidbody>();
            for (int i = 0; i < RagdollRoot.childCount; i++)
            {
                GetComp<Rigidbody>(RagdollRoot.GetChild(i),allRbs);
            }
            colliders = new List<Collider>();
            for (int i = 0; i < RagdollRoot.childCount; i++)
            {
                GetComp<Collider>(RagdollRoot.GetChild(i),colliders);
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
            GameManager.Instance.sounds.PlaySingleTime(Sounds.WallHit);
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
                GameManager.Instance.sounds.PlaySingleTime(Sounds.DummyCollision);
            }
            GameManager.Instance.eventManager.StrongDummyImpact.RemoveListener(OnStrongCollision);
        }




        private void OnDisable()
        {
            if(checkToken != null)
                checkToken.Cancel();
        }

    }



}