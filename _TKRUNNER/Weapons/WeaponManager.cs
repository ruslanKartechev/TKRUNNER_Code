using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace TKRunner
{
    public class WeaponManager : MonoBehaviour, IDraggable
    {
        [SerializeField] protected Collider mColl;
        [SerializeField] protected Collider ModelCollider;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Rigidbody RB;
        [Space(10)]
        public WeaponType _type;
        [Space(5)]
        public float Size = 1f;
        [Space(5)]
        [Header("Weapon")]
        [SerializeField] protected Weapon _weapon; 
        [Header("Parts")]
        [SerializeField] protected Transform  pieces;
        [Header("SplinePosition")]
        [SerializeField] protected SplinePositionHandler _splineHandler;
        [Header("Correct Angles when is active")]
        [SerializeField] protected Vector3 ActiveAngles;
        [Header("Aura settgins")]
        [SerializeField] protected GameObject Aura;
        [SerializeField] protected Renderer AuraRend;
        [SerializeField] protected float FadeTime = 0.4f;
        [Header("DragPlane Height")]
        [SerializeField] private float DragPlaneHeight = 1f;

        protected Coroutine Flying;
        protected Coroutine LifeCountdown;

        protected Action breakDrag;
        protected IBreakable _breakble;

        protected Coroutine rotationHandler = null;
        private void Start()
        {
            if (mColl == null)
                mColl = GetComponent<Collider>();
            if (RB == null)
                RB = GetComponent<Rigidbody>();

            ModelCollider.enabled = false;
            RB.constraints = RigidbodyConstraints.None;
            RB.useGravity = false;
            RB.isKinematic = true;
            _weapon.BreakWeapon = BreakWeapon;
            if(pieces != null)
                _breakble = pieces.gameObject.GetComponent<IBreakable>();
        }
        public void DragStart(DragPlane dragPlane, Action Break)
        {
            StartCoroutine(HideAura());
            transform.eulerAngles = ActiveAngles;
            RB.isKinematic = false;
            RB.useGravity = true;
            Init();
            _weapon.Activate();
            breakDrag = Break;
            rotationHandler = StartCoroutine(RotationHandler());
            GameManager.Instance.data.currentWeapon = _type;
            GameManager.Instance.eventManager.WeaponEquipped.Invoke();
        }
        public void DragEnd()
        {
            animator.Play(AnimNames.WeaponThrown);
            _weapon.DeActivate();
            if (mColl != null)
                mColl.isTrigger = false;
            RB.constraints = RigidbodyConstraints.None;
            breakDrag = null;
            if(rotationHandler != null)
                StopCoroutine(rotationHandler);
            GameManager.Instance.data.currentWeapon = WeaponType.Default;
            GameManager.Instance.eventManager.WeaponEquipped.Invoke();
        }

        private void BreakWeapon()
        {
            GameManager.Instance.sounds.PlaySingleTime(Sounds.WeaponBreak);
            BreakConnection();
            ModelCollider.gameObject.SetActive(false);
            _breakble?.Break();
        }

        public void BreakConnection()
        {
            if (breakDrag != null)
                breakDrag.Invoke();
            breakDrag = null;
        }
        public Rigidbody GetRigidBody()
        {
            return RB;
        }

        public bool CreateDragAura()
        {
            return false;
        }

        public Vector3 AuraOffset()
        {
            return Vector3.zero;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        public bool AllowDrag()
        {
            return true;
        }

        private IEnumerator RotationHandler()
        {
            while (true)
            {
                if(_splineHandler.GetTargetPercent(transform.position) >= _splineHandler.GetPlayerPercent())
                    transform.rotation = Quaternion.LookRotation(GameManager.Instance.data.Player.gameObject.transform.forward);
                else
                    transform.rotation = Quaternion.LookRotation( -GameManager.Instance.data.Player.gameObject.transform.forward );
                yield return null;
            }
        }

        protected virtual void Init()
        {
            _splineHandler.Init();
            ModelCollider.enabled = true;
            animator.Play(AnimNames.WeaponActive,0,0);
            RB.constraints = RigidbodyConstraints.FreezeRotation;
        }
        public float GetPlaneHeight()
        {
            return DragPlaneHeight;
        }


        protected virtual IEnumerator HideAura()
        {
            float start = -1;
            float end = 2;
            float elapsed = 0f;
            float scale_1 = Aura.transform.localScale.x;
            float scale_2 = 0.1f;
            while(elapsed <= FadeTime)
            {
                AuraRend.material.SetFloat("_FresnelDepth", Mathf.Lerp(start,end,elapsed/ FadeTime));
                Aura.transform.localScale = Vector3.one * Mathf.Lerp(scale_1, scale_2, elapsed / FadeTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Aura.SetActive(false);
        }

        public float GetSize()
        {
            return Size;
        }

        public Color GetColor()
        {
            return Color.white;
        }
    }
}