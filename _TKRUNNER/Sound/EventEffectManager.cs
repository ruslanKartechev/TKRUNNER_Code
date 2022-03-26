using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
namespace Commongame.Sound {
    public class EventEffectManager : MonoBehaviour
    {

        public SoundNames levelStart;
        public SoundNames levelEnd;
        public SoundNames win;
        public SoundNames loose;
        public SoundNames lazerApproached;
        public SoundNames portalApproached;

        private void Start()
        {
            GameManager.Instance.eventManager.LevelStarted.AddListener(OnLevelStart);
            GameManager.Instance.eventManager.PlayerWin.AddListener(OnPlayerWin);
            GameManager.Instance.eventManager.PlayerLose.AddListener(OnPlayerLoose);
            GameManager.Instance.eventManager.LevelEndreached.AddListener(OnLevelEnd);
        }


        private void OnLevelStart()  
        {
            GameManager.Instance.sounds.StopLoopedEffect(levelEnd.ToString());
            GameManager.Instance.sounds.PlayMusic();
        }
        private void OnLevelEnd()
        {
            GameManager.Instance.sounds.StopMusic();
        }
        private void OnPlayerWin()
        {
            //List<string> names = new List<string>(2);
            //names.Add(win.ToString());
            //names.Add(levelEnd.ToString());
            //GameManager.Instance.sounds.PlaySequence(names);
            GameManager.Instance.sounds.PlaySingleTime(win.ToString());
            GameManager.Instance.sounds.StartSoundEffect(levelEnd.ToString());
        }
        private void OnPlayerLoose()
        {
            GameManager.Instance.sounds.PlaySingleTime(loose.ToString());
        }
        private void InSourceVicinity(SoundNames sound, bool play)
        {
            Debug.Log("vicinity of: " + sound.ToString());
            if (play)
                GameManager.Instance.sounds.StartSoundEffect(sound.ToString());
            else
                GameManager.Instance.sounds.StopLoopedEffect(sound.ToString());
        }



    }
}