using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Commongame
{
    public interface ITwoStepEffect
    {
        void OnOpen();
        void OnClose();
    }


    public class ElevatorManager : MonoBehaviour, ITwoStepEffect
    {
        public Animator mAnim;
        public ISoundEffect _sound;
        public string _DoorCloseAnimName;
        public string _DoorOpenAnimName;

        private void Start()
        {
            _sound = GetComponent<ISoundEffect>();
            
        }

        #region ITwoStepEffect
        public void OnOpen()
        {
            OpenDoors();
        }
        public void OnClose()
        {
            CloseDoors();
         //   mAnim.Play("Idle", 0, 0);
        }
        #endregion



        public void OpenDoors()
        {
            mAnim.Play(_DoorOpenAnimName, 0, 0);
        }
        public void CloseDoors()
        {
            mAnim.Play(_DoorCloseAnimName, 0, 0);   
        }
        public void OnDoorsOpen()
        {

        }
        public void OnDoorsClose()
        {
            _sound?.PlayEffectOnce();
        }

    }
}