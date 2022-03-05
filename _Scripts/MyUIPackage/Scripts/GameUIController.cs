using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Events;
using General;

namespace General.UI
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
        }
        public void ShowStart()
        {
            levelEndPanel.HidePanel(true);
            startPanel.ShowPanel();
        }

        public void OnLevelFinishReached()
        {
            levelEndPanel.ShowPanel();
        }

    }
}