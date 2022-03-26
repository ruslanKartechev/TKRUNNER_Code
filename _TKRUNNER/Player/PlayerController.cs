using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;



namespace TKRunner
{

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEdtitor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PlayerController me = (PlayerController)target;
            if (GUILayout.Button("Win"))
            {
                me.AutoWin();
            }
        }
    }

#endif

    public class PlayerController : MonoBehaviour,IDamagable
    {
        public PlayerManager manager;
        public PlayerIKManager IKmanager;
        public PlayerDragControllRus dragController;
        public PlayerHealthManager health;
        public PlayerRagdollManager ragdoll;
        public bool ShakeOnDamage = false;
        [Space(10)]
        public Transform WinCamPLace;
        public Transform LooseCamPlace;

        public float currentPercent { get { return manager.CurrentPercent; } }
        public float currentSpeed { get { return manager.GetSpeed(); } }
        [SerializeField] private Transform _geometryCenter;
        public Transform GeometryCenter { get { return _geometryCenter; } }
        bool allowDamage = true;

#if UNITY_EDITOR
        public void AutoWin()
        {
            Transform finish = GameObject.FindGameObjectWithTag(Tags.LevelEnd).transform;
            OnPlayerWin();
            transform.position = finish.position;
            GameManager.Instance.eventManager.PlayerWin.Invoke();
        }

#endif 




        private void Start()
        {
            if (manager == null)
                manager = GetComponent<PlayerManager>();
            if (health == null)
                health = GetComponent<PlayerHealthManager>();
            if (dragController == null)
                dragController = GetComponent<PlayerDragControllRus>();

            manager.Init(this);
            IKmanager.Init(this);
            IKmanager.SetWeight(0);
            dragController.Init(this, manager);

            GameManager.Instance.eventManager.LevelLoaded.AddListener(OnLevelLoaded);
            GameManager.Instance.eventManager.LevelStarted.AddListener(OnLevelStart);
            GameManager.Instance.eventManager.LevelEndreached.AddListener(OnLevelEnd);
            GameManager.Instance.eventManager.PlayerWin.AddListener(OnPlayerWin);
            GameManager.Instance.data.Player = this;
        }

        private void OnLevelLoaded()
        {
            health.Init(this, GameManager.Instance.data.currentInst.Data._PlayerHealth.MaxHealth);
            manager.InitActive(GameManager.Instance.data.currentInst.playerSpline);
            GameManager.Instance.data.currentInst.WinCamPos = WinCamPLace;
            GameManager.Instance.data.currentInst.LooseCam = LooseCamPlace;
        }

        private void OnLevelStart()
        {
            dragController.AllowDrag();
            manager.StartMoving();
            manager.TurnForward();
        }

        private void OnLevelEnd()
        {
            dragController.DisallowDrag();
            manager.StopMoving();
        }

        private void OnPlayerWin()
        {
            dragController.BreakDrag();
            manager.TurnBackwards(false);
            manager.StopRotationCountdown();
            manager.PlayWinAnim();
            allowDamage = false;


        }
        public void TakeHit()
        {
            if (allowDamage == false) return;
            if (health.DoDie() == false)
            {
                OnDamage();
            }
            else
            {
                OnDeath();
            }
            if (ShakeOnDamage)
                GameManager.Instance.eventManager.Impact.Invoke();

        }
        public void OnDamage()
        {
            allowDamage = false;
            IKmanager.SetWeight(0);
            manager.OnDamage();
            dragController.DisallowDrag();
            
        }
        public void Restore()
        {
            IKmanager.SetWeight(0);
            allowDamage = true;
            dragController.AllowDrag();
        }
        public void OnDeath()
        {
            dragController.BreakDrag();
            dragController.DisallowDrag();
            manager.OnDeath();
            ragdoll.SetActive();
            IKmanager.SetWeight(0);
            GameManager.Instance.eventManager.PlayerLose.Invoke();
        }



        


    }


}