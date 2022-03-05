using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Events;
using General.Sound;
using General.Data;
using General.UI;
using General.Cam;
using TKRunner;
namespace General
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
       [HideInInspector] public GameUIController UIController;
        [Header("Dummies")]
        public DummySpawnerController dummySpawner;
        public DummyController dummyController;
        [Header("_")]
        public CameraController CameraController;


        private void Start()
        {
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
