using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
namespace Commongame.Sound
{

    public class SoundEffectManager : MonoBehaviour
    {
        [Header("Sound Data")]
        [SerializeField] private SoundData sounds;
        
        [SerializeField] private SoundSourceManager sourceManager;
        [SerializeField] private SoundLoopHandler soundLooper;
        [Tooltip("AutoPlay music on start")]
        [SerializeField] private bool playMusic = true;
        [Tooltip("Prohibit same sound to be played on multiple loops")]
        [SerializeField] private bool DisallowMultipleLoops = true;
        [Range(0f,1f)]
        [SerializeField] private float musicVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float effectsVolume = 1f;
        public float MusicVolume { get { return musicVolume; }}
        public float EffectsVolume { get { return effectsVolume; } }

        public void Init()
        {
            if (sounds == null)
                Debug.Log("</color=red>SoundData is not assigned </color>");
            sourceManager.Init();
            sourceManager.CreateSources(SourceByName.EffectsSource, 20);
            sourceManager.CreateMusicSources(SourceByName.MusicSource, 1);
            if (soundLooper == null)
                soundLooper = GetComponent<SoundLoopHandler>();
            if (playMusic)
                PlayMusic();
        }
        public void StartSoundEffect(string soundName)
        {
            AudioSource source = sourceManager.FindFreeSource(SourceByName.EffectsSource).source;
            StartSoundEffect(soundName, source);

        }
        public void StopLoopedEffect(string soundName)
        {
            soundLooper.StopLoop(soundName);
        }
        
        public void PlaySingleTime(string soundName)
        {
            Sound sound = sounds.soundEffects.Find(x => x.name == soundName);
            if (sound == null)
                return;
            AudioSource source = sourceManager.FindFreeSource(SourceByName.EffectsSource).source;
            if (source == null )
                return;
            source.pitch = sound.pitch;
            source.volume = sound.volume * EffectsVolume;
            source.PlayOneShot(sound.clip);
        }
  
        public void StartSoundEffect(string soundName, AudioSource source)
        {
            if (source == null)
                return;
            Sound sound = sounds.soundEffects.Find(x => x.name == soundName);
            if (DisallowMultipleLoops == true)
            {
                if (soundLooper.IsPlayingLoop(sound) == true)
                    return;
            }
            soundLooper.StartLoop(source, sound, EffectsVolume);
        }



        public void PlaySingleTime(string soundName, AudioSource source)
        {
            if (source == null)
                return;
            Sound sound = sounds.soundEffects.Find(x => x.name == soundName);
            if (sound.clip == null)
                return;
            source.pitch = sound.pitch;
            source.volume = sound.volume * EffectsVolume;
            source.PlayOneShot(sound.clip);
        }


        public void PlaySequence(List<string> OrderedNames)
        {
            StartCoroutine(PlayingSequentially(OrderedNames));
        }
        private IEnumerator PlayingSequentially(List<string> OrderedNames)
        {
            AudioSource source = sourceManager.FindFreeSource(SourceByName.EffectsSource).source;
            foreach (string name in OrderedNames)
            {
                Sound sound = sounds.soundEffects.Find(x => x.name == name);
                if (sound == null)
                    Debug.Log("sound is null");
                source.pitch = sound.pitch;
                source.volume = sound.volume * EffectsVolume;
                source.PlayOneShot(sound.clip);
                yield return new WaitForSeconds(sound.clip.length);

            }
        }



        #region Music
        public void PlayMusic()
        {
            if (sounds.music != null && sounds.music.Count > 0)
            {
                StartCoroutine(MusicPlaying());
            }
        }
        private IEnumerator MusicPlaying()
        {
            SoundSource soundSource = sourceManager.FindFreeMusicSource(SourceByName.MusicSource);
            if (soundSource == null) yield break ;

            AudioSource source = soundSource.source;
            while (true)
            {
                if (source.isPlaying == false)
                {
                    Sound sound= SetRandomMusic(source);
                    if (sound == null)
                        yield break;
                    yield return new WaitForSeconds(sound.clip.length);
                }
                yield return null;
            }

           
        }
        private Sound SetRandomMusic(AudioSource source)
        {
            Sound sound = sounds.music[Random.Range(0, sounds.music.Count)];
            if (sound.clip == null)
                return null;
            source.clip = sound.clip;
            source.pitch = sound.pitch;
            source.volume = sound.volume * MusicVolume;
            source.PlayOneShot(source.clip);
            return sound;
        }


        public void StopMusic()
        {
            foreach(SoundSource source in sourceManager.MusicSources)
            {
                source.source.Stop();
            }
        }


        #endregion

        public void OffMusic()
        {
            musicVolume = 0;
            InitSoundSources();
        }
        public void OffEffects()
        {
            effectsVolume = 0;
            InitSoundSources();
        }
        public void OnMusic()
        {
            musicVolume = 1;
            InitSoundSources();
        }
        public void OnEffects()
        {
            effectsVolume = 1;
            InitSoundSources();
        }
        private void InitSoundSources()
        {
            foreach(SoundSource source in sourceManager.SoundSources)
            {
                if(source.name == SourceByName.EffectsSource)
                    source.source.volume *= EffectsVolume;
                else if(source.name == SourceByName.MusicSource)
                    source.source.volume *= MusicVolume;
            }
        }

    }
}