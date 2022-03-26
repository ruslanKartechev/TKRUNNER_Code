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
        public SoundEffectManager sounds;
        public EventsManager eventManager;
        public DataManager data;
        public DataLoader dataLoader;
        public Controlls controlls;
        public GoClearer _clearer;

        [HideInInspector] public GameUIController UIController;
        
        [Header("Dummies")]
        public DummySpawnerController dummySpawner;
        public DummyController dummyController;
        [Header("_")]
        public CameraController CameraController;


        private void Start()
        {

            //levelManager._Loader.ClearLevel();

            UIController = FindObjectOfType<GameUIController>();
            levelManager = FindObjectOfType<LevelManager>();


            controlls.Init();
            UIController.Init();
            dummySpawner.Init();
            sounds.Init();
          //  CameraController.Init();
            GameManager.Instance.eventManager.DataLoaded.AddListener(OnDataLoaded);
            GameManager.Instance.eventManager.LevelLoaded.AddListener(OnLevelLoaded);
            GameManager.Instance.eventManager.NextLevelCalled.AddListener(OnNextLevelCalled);
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
           // GameManager.Instance.eventManager.LevelStarted.Invoke();
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
