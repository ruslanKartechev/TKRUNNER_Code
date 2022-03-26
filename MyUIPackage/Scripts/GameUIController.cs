using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Events;
using Commongame;

namespace Commongame.UI
{
    public class GameUIController : MonoBehaviour
    {
        public StartPanel startPanel;
        public LevelCompletePanel levelEndPanel;
        public void Init()
        {
            if (startPanel == null)
                startPanel = GetComponent<StartPanel>();
            if (levelEndPanel == null)
                levelEndPanel = GetComponent<LevelCompletePanel>();

            startPanel.Init();
            levelEndPanel.Init();
            startPanel.HidePanel(true);
            levelEndPanel.HidePanel(true);

            GameManager.Instance.eventManager.LevelLoaded.AddListener(ShowStart);
            GameManager.Instance.eventManager.LevelEndreached.AddListener(OnLevelFinishReached);
            GameManager.Instance.eventManager.PlayerLose.AddListener(OnPlayerLoose);
        }
        public void ShowStart()
        {
            levelEndPanel.HidePanel(true);
            startPanel.ShowPanel();
        }
        public void OnPlayerLoose()
        {
            levelEndPanel.ShowRetryPanel();
        }
        public void OnLevelFinishReached()
        {
            levelEndPanel.ShowPanel();
        }

    }
}