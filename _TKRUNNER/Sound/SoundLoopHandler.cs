using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Commongame.Sound
{
    [System.Serializable]
    public class LoopedSound
    {
        public string name;
        public Coroutine myLoop;
        public AudioSource source;
        public LoopedSound(string _name, Coroutine _myLoop, AudioSource _source)
        {
            name = _name;
            myLoop = _myLoop;
            source = _source;
        }
    }
    public class SoundLoopHandler : MonoBehaviour
    {
        public List<LoopedSound> activeLoops = new List<LoopedSound>();


        public void StartLoop(AudioSource source, Sound sound, float volume)
        {
            LoopedSound myLoop = activeLoops.Find(x => x.name == sound.name);
            if(myLoop != null)
            {
                if (myLoop.myLoop != null)
                    StopCoroutine(myLoop.myLoop);
                activeLoops.Remove(myLoop);
            }

            Coroutine looping =  StartCoroutine(PlayOnLoop(source, sound, volume));
            activeLoops.Add(new LoopedSound(sound.name, looping, source));
        }

        public void StopLoop(string SoundName)
        {
            LoopedSound myLoop = activeLoops.Find(x => x.name == SoundName);
            if (myLoop != null)
            {
                if(myLoop.myLoop != null)
                    StopCoroutine(myLoop.myLoop);
                myLoop.source.Stop();
                activeLoops.Remove(myLoop);
            }
                
        }
        public IEnumerator PlayOnLoop(AudioSource source, Sound sound, float volume)
        {
            source.clip = sound.clip;
            source.pitch = sound.pitch;
            source.volume = sound.volume * volume;
            source.loop = false;
            source.Play();


            if(sound.loopedTime_start == 0  &&  sound.loopedTime_end == 0)
            {
                while (true)
                {
                    yield return new WaitForSeconds(sound.clip.length);
                    source.time = 0;
                }
            }
            else
            {
                yield return new WaitForSeconds(sound.loopedTime_start);
                while (true)
                {
                    source.time = sound.loopedTime_start;
                    source.Play();
                    yield return new WaitForSeconds(sound.loopedTime_end - sound.loopedTime_start);

                }
            }

            

        }


        public bool IsPlayingLoop(Sound sound)
        {
            LoopedSound s = activeLoops.Find(x => x.name == sound.name);
            if (s == null)
                return false;
            else
                return true;
        }


    }

}