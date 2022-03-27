using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Sound;
using Commongame;
using System;
namespace TKRunner
{
    public class BaseObstacle : MonoBehaviour, IDestroyable
    {
        [Tooltip("Check collisions on start, without dragging")]
        [SerializeField] private bool NoDragCollisions = false;
        public bool AllowDragging = true;
        [Space(5)]
        [SerializeField] protected Rigidbody RB;
        [SerializeField] protected float PushForceMultiplyer = 1f;
        [Space(10)]
        [SerializeField] protected int MaxHits;
        [Space(10)]
        [SerializeField] protected float _Size = 0.02f;
        public float Size { get { return _Size * transform.localScale.y; } } 
        [Space(10)]
        [SerializeField] protected GameObject PiecesRoot;
        [SerializeField] protected BaseObstacleDragTarget DragTarget;
        [Space(10)]
        [SerializeField] protected Renderer _rend;
        [SerializeField] protected Collider _coll;
        public Color RendColor { get { return _rend.material.GetColor("_BaseColor");; } }
        [Space(5)]
        [SerializeField] private SoundNames breakSound;

        protected float DragColliderScaleFactor = 1.35f;

        protected IBreakable _breakble;
        protected int Hits = 0;
        private Action _playerContact;
        private Action<Transform> _enemyContact;
        private void Start()
        {
            RB.mass = 50;
            if (NoDragCollisions == true)
            {
                _enemyContact = OnEnemyContactPassive;
                _playerContact = OnPlayerContact;
            }
    
            breakSound = SoundNames.BoxBreak;
            _breakble = PiecesRoot.GetComponent<IBreakable>();
            DragTarget?.Init(this);
        }

        protected virtual void AddHitCount()
        {
            Hits++;
            if(Hits >= MaxHits)
            {
                Break();
            }
        }
        protected virtual void Break()
        {
            DragTarget.BreakConnection();
            _breakble?.Break();
            GameManager.Instance._sounds.PlaySingleTime(breakSound.ToString());
            gameObject.SetActive(false);
        }
        public virtual void OnDragStart()
        {
            _playerContact = null;
            //Debug.Log("On drag start. OnEnemyContact Action registered");
            _enemyContact = OnEnemyContanctDrag;
            NoDragCollisions = false;
        }
        public virtual void OnDragEnd()
        {
            
        }


        private void OnEnemyContanctDrag(Transform hit)
        {
            IWeaponTarget target = hit.gameObject.GetComponent<IWeaponTarget>();
            if (target == null) { Debug.Log("target has no IWeaponTarget, abort"); return; }
            Vector3 vel = GameManager.Instance._data.CurrentDragVelocity;
            vel.y = 0;
            //Debug.Log("<color=blue> Obstacle OnEnemyContanctDrag called  </color>");
            //Debug.Log("<color=blue> +Speed = " + vel.magnitude + 
            //    " Min: " + GameManager.Instance.data.currentInst.Data._collisionData.BoxCollisionForceThreshold 
            //    +  " </color>");
            target.KillAndPush((vel) * PushForceMultiplyer);
            AddHitCount();
            if (vel.magnitude >= GameManager.Instance._data.currentInst.Data._collisionData.BoxCollisionForceThreshold)
            {
               
            }
        }
        private void OnEnemyContactPassive(Transform hit)
        {
            IWeaponTarget target = hit.gameObject.GetComponent<IWeaponTarget>();
            if (target == null) return;
            target.KillAndPush(hit.position + transform.position);
            AddHitCount();
        }
        private void OnPlayerContact()
        {
            //Debug.Log("<color=blue> Obstacle OnPlayerContact called  </color>");
            GameManager.Instance._data.Player.TakeHit();
            Break();

        }
        public void Destroy()
        {
            Break();
        }

        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.collider.tag)
            {
                case Tags.NormalDummy:
                    //Debug.Log("<color=blue> Obstacle Collision registered  </color>");
                    _enemyContact?.Invoke(collision.transform);
                    break;
                case Tags.Player:
                    _playerContact?.Invoke();
                    break;
            }
        }




      



    }
}