using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
namespace Commongame.UI
{
    public class LevelCompletePanel : UIPanelManager
    {
        [SerializeField] private LevelCompletePanelUI panelUI;

        public void Init()
        {
            panelUI.Init(this);
            GameManager.Instance.eventManager.LevelLoaded.AddListener(OnNewLevel);
            mPanel = panelUI;
        }
        public void OnNewLevel()
        {
            int level = GameManager.Instance.levelManager.CurrentLevelIndex + 1;
            panelUI.SetLevel(level);
        }

        public override void ShowPanel(bool showButton = true)
        {
            base.ShowPanel(false);

        }



        public override void OnMainButtonClick()
        {
            HidePanel(false);
            GameManager.Instance.eventManager.NextLevelCalled.Invoke();
        }

        public void ShowRetryPanel()
        {
            panelUI.gameObject.SetActive(true);
            int level = GameManager.Instance.levelManager.CurrentLevelIndex + 1;
            panelUI.SetHeaderText("Level " + level + " Failed");
            panelUI.StartHeaderAnimator();
            panelUI.HideMainButton(false);
            panelUI.ShowRetryButton();

        }

        public void OnRetryButtonClick()
        {
            GameManager.Instance.levelManager.RestartLevel();
        }
    }
}