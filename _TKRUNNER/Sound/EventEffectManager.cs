using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Sound;
namespace TKRunner {
    public class EventEffectManager : MonoBehaviour
    {

        public SoundNames levelStart;
        public SoundNames levelEnd;
        public SoundNames win;
        public SoundNames loose;

        private void Start()
        {
            GameManager.Instance._events.LevelStarted.AddListener(OnLevelStart);
            GameManager.Instance._events.PlayerWin.AddListener(OnPlayerWin);
            GameManager.Instance._events.PlayerLose.AddListener(OnPlayerLoose);
            GameManager.Instance._events.LevelLoaded.AddListener(OnLevelLoaded);
        }


        private void OnLevelLoaded()
        {
            GameManager.Instance._sounds.StopLoopedEffect(levelEnd.ToString());
            GameManager.Instance._sounds.PlayMusic();
        }

        private void OnLevelStart()  
        {
            GameManager.Instance._sounds.StopLoopedEffect(levelEnd.ToString());
        }
        private void OnPlayerWin()
        {

            GameManager.Instance._sounds.StopMusic();
            GameManager.Instance._sounds.PlaySingleTime(win.ToString());
            GameManager.Instance._sounds.StartSoundEffect(levelEnd.ToString());
        }
        private void OnPlayerLoose()
        {
            GameManager.Instance._sounds.StopMusic();
            GameManager.Instance._sounds.PlaySingleTime(loose.ToString());
        }
        private void InSourceVicinity(SoundNames sound, bool play)
        {
            Debug.Log("vicinity of: " + sound.ToString());
            if (play)
                GameManager.Instance._sounds.StartSoundEffect(sound.ToString());
            else
                GameManager.Instance._sounds.StopLoopedEffect(sound.ToString());
        }



    }
}