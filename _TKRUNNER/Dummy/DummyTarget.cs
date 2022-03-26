using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using System.Threading;
using System.Threading.Tasks;
using System;
namespace TKRunner 
{
    [DisallowMultipleComponent]
    public class DummyTarget : MonoBehaviour
    {
        //[SerializeField] private bool guarded = false;
        private DummyManager manager;
        [SerializeField] private Rigidbody TargetRB;
        [Header("Collider")]
        public Collider DragCollider;
        [Header("AimTarget")]
        [SerializeField] private SpriteRenderer AimSpritePF;
        [SerializeField] private Transform AimPlace;
        [SerializeField] private float Scale = 0.05f;
        [Header("Dragging")]
        [SerializeField] private float DragPlaneHeight;
        [Header("Aura")] 
        [SerializeField] private Vector3 auraOffset = new Vector3();


        private SpriteRenderer aimInst;
        private CancellationTokenSource animToken;
        private Action breaking;

        public bool AllowDrag()
        {
            if (manager.CurrentState == DummyStates.Run || manager.CurrentState == DummyStates.Truck)
                return true;
            else 
                return false;
        }
        public bool CreateDragAura() { return true; }

        public Vector3 AuraOffset() { return auraOffset; }


        public void Init(DummyManager _manager)
        {
            manager = _manager;
        }
        public void DragStart(DragPlane dragPlane, Action Break)
        {

            breaking = Break;
        }
        public void DragEnd()
        {

            breaking = null;
        }

        public void BreakConnection()
        {
            breaking?.Invoke();
        }


        public GameObject GetGameObject() { return gameObject; }

        public bool IsActive() { return gameObject.activeInHierarchy; }

        public float GetPlaneHeight()
        {
            return DragPlaneHeight;
        }
        public void FlyToTarget(Transform target, float speed)
        {
           // manager.FlyToTarget(target, speed);
        }
        public Rigidbody GetRigidBody()
        {
            return TargetRB;
        }




        public void SetNonDraggable()
        {
            if (DragCollider == null) Debug.Log("No drag Collider");
            else
                DragCollider.enabled = false;
        }

        public void ShowAim()
        {
            if (aimInst == null)
            {
                aimInst = Instantiate(AimSpritePF);
                aimInst.gameObject.transform.parent = AimPlace;
                aimInst.gameObject.transform.localPosition = Vector3.zero;
                aimInst.transform.localEulerAngles = Vector3.zero;
                aimInst.gameObject.transform.localScale = Vector3.one * Scale;
            }
            else
            {
                aimInst.gameObject.SetActive(true);
            }
            if (animToken != null)
                animToken.Cancel();
            animToken = new CancellationTokenSource();
            AimHandler();
            AimRotation();
        }
        public void HideAim()
        {
            animToken.Cancel();
            if (animToken != null)
                animToken.Cancel();
            if (aimInst != null)
                aimInst.gameObject.SetActive(false);
            aimInst = null;
        }

        private float ScaleTime = 0.25f;
        private float rotationSpeed = 100;
        private async void  AimHandler()
        {
            while (aimInst != null && aimInst.gameObject.activeInHierarchy
                && animToken.Token.IsCancellationRequested == false)
            {
                if(aimInst != null)
                    await Scaling(aimInst.transform, ScaleTime, Scale*1.5f);
                if (aimInst != null)
                    await Scaling(aimInst.transform, 2* ScaleTime, Scale*0.5f);
                if (aimInst != null)
                    await Scaling(aimInst.transform, ScaleTime, Scale);
                await Task.Yield();
            }
        }
        private async void AimRotation()
        {
            while (aimInst != null && aimInst.gameObject.activeInHierarchy
        && animToken.Token.IsCancellationRequested == false)
            {
               aimInst.transform.parent.transform.LookAt(Camera.main.transform, Vector3.up);

                aimInst.transform.localEulerAngles +=
                    new Vector3(0,0,rotationSpeed * Time.deltaTime);
                await Task.Yield();
            }
        }
        private async Task Scaling(Transform target, float time, float endFactor)
        {
            float elapsed = 0f;
            float startFactor = target.localScale.x;
            while(elapsed <= time && animToken.Token.IsCancellationRequested == false)
            {
                target.localScale = Mathf.Lerp(startFactor, endFactor, elapsed / time) * Vector3.one;
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            target.localScale = Vector3.one * endFactor;
        }



        private void OnDisable()
        {
            if (animToken != null)
                animToken.Cancel();
        }

        public float GetSize()
        {
            throw new NotImplementedException();
        }

        public Color GetColor()
        {
            throw new NotImplementedException();
        }
    }
}