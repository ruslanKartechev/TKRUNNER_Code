using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
namespace General.Sound
{
    public class SoundSource
    {
        public AudioSource source;
        public string name;

    }

    public class SoundSourceManager : MonoBehaviour
    {
        private List<SoundSource> _Sources = new List<SoundSource>();
        private List<SoundSource> musicSources = new List<SoundSource>();
        public List<SoundSource> SoundSources { get { return _Sources; } }
        public List<SoundSource> MusicSources { get { return musicSources; } }
        [SerializeField] private GameObject sourceObject;

        public void Init(GameObject soundSource = null)
        {
            if (sourceObject == null && soundSource == null)
                sourceObject = gameObject; // changable
            else if(sourceObject == null && soundSource != null)
                sourceObject = soundSource;
        }

        

        public List<SoundSource> CreateSources(string name, int amount)
        {
            if (sourceObject == null)
                Debug.Log("source object null");
            _Sources = new List<SoundSource>(amount);
            for(int i=0; i<amount; i++)
            {
              AppedSoundSoure(name + " " + (i + 1).ToString(), SoundSources);
            }
          
            return _Sources;
        }

        public List<SoundSource> CreateMusicSources(string name, int amount)
        {
            musicSources = new List<SoundSource>(amount);
            for (int i= 0; i < amount; i++)
            {
                AppedSoundSoure(name + " " + i.ToString(), musicSources);
            }
            return musicSources;
        }

        private void AppedSoundSoure(string name, List<SoundSource> output)
        {
            SoundSource _source = new SoundSource();
            _source.name = name;
            _source.source = sourceObject.AddComponent<AudioSource>();
            output.Add(_source);
        }

        public SoundSource FindFreeSource(string name)
        {
            SoundSource source = FindFreeSource(name, _Sources);
            return source;
        }
        public SoundSource FindFreeMusicSource(string name)
        {
            SoundSource source = FindFreeSource(name, MusicSources);
            return source;
        }
        private SoundSource FindFreeSource(string name, List<SoundSource> searchAt)
        {
            SoundSource source = null;
            for (int i = 0; i < searchAt.Count; i++)
            {
                if (searchAt[i].name.Contains(name) && searchAt[i].source.isPlaying == false)
                {
                    return searchAt[i];
                }
            }
            Debug.Log("free source was not found");
            return source;
        }
    }
}