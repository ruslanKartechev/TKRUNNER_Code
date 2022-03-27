using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Events;
using Commongame.Sound;
using Commongame.Data;
using Commongame.UI;
using Commongame.Cam;
using TKRunner;
namespace Commongame
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : SingletonMB<GameManager>
    {
        [Header("Debugging")]
        public bool DoStartGame = true;
        [HideInInspector]  public LevelManager levelManager;
        [Header("General")]
        public SoundEffectManager _sounds;
        public EventsManager _events;
        public DataManager _data;
        public DataLoader dataLoader;
        public Controlls _controls;
        public GoClearer _clearer;

        [HideInInspector] public GameUIController _UI;
        
        [Header("Dummies")]
        public DummyController dummyController;
        [Header("_")]
        public CameraController CameraController;

        private void Start()
        {
            _UI = FindObjectOfType<GameUIController>();
            levelManager = FindObjectOfType<LevelManager>();

            _controls.Init();
            _UI.Init();
            _sounds.Init();
            GameManager.Instance._events.DataLoaded.AddListener(OnDataLoaded);
            GameManager.Instance._events.LevelLoaded.AddListener(OnLevelLoaded);
            GameManager.Instance._events.NextLevelCalled.AddListener(OnNextLevelCalled);
            dataLoader.StartLoading();
        }
        public void OnDataLoaded()
        {
            if (DoStartGame)
            {
                levelManager.InitLevel(levelManager.CurrentLevelIndex);
            }
        }

        private void OnLevelLoaded()
        {
        }
        private void OnNextLevelCalled()
        {
            levelManager.NextLevel();
        }


    }



}




public class SingletonMB<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<T>();
            return instance;
        }
        set
        {
            instance = value;
        }
    }



}
