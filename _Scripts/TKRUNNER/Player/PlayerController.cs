using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
using General;
using System.Threading;
using System.Threading.Tasks;
namespace TKRunner
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerManager manager;
        public PlayerIKManager IKmanager;
        public PlayerDragControllRus dragController;
        public PlayerHealthManager health;
        public float currentPercent { get { return manager.CurrentPercent; } }
        public float currentSpeed { get { return manager.GetSpeed(); } }


        private void Start()
        {
            if (manager == null)
                manager = GetComponent<PlayerManager>();
            if (health == null)
                health = GetComponent<PlayerHealthManager>();
            if (dragController == null)
                dragController = GetComponent<PlayerDragControllRus>();


            IKmanager.Init(this);
            IKmanager.SetWeight(0);
            dragController.Init(this, manager);
            health.Init(this);
            GameManager.Instance.eventManager.LevelLoaded.AddListener(OnLevelLoaded);
            GameManager.Instance.eventManager.LevelStarted.AddListener(OnLevelStart);
            GameManager.Instance.eventManager.LevelEndreached.AddListener(OnLevelEnd);
           // GameManager.Instance.eventManager.PlayerLose.AddListener(OnPlayerLose);
            GameManager.Instance.eventManager.PlayerWin.AddListener(OnPlayerWin);
            GameManager.Instance.data.Player = this;
        }

        private void OnLevelLoaded()
        {
            manager.InitActive(GameManager.Instance.data.currentInst.levelSpline);
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
           
        }
       
        public void OnDeath()
        {
            dragController.BreakDrag();
            manager.OnDeath();
        }

        public void TakeHit()
        {
            health.TakeHit();

        }

        


    }


}