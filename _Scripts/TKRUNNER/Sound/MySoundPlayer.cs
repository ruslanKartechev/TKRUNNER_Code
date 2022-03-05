using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
using General;
public class MySoundPlayer : MonoBehaviour, ISoundEffect
{
    public SoundNames _sound;  
  //  [SerializeField] private string mySoundName;

    public void PlayEffectOnce()
    {

        GameManager.Instance.sounds.PlaySingleTime(_sound.ToString());
    }

    public void StartEffect()
    {
        GameManager.Instance.sounds.StartSoundEffect(_sound.ToString());
    }

    public void StopEffect()
    {
        GameManager.Instance.sounds.StopLoopedEffect(_sound.ToString());
    }
}
