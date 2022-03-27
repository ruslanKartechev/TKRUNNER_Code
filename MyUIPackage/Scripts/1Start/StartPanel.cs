using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
namespace Commongame.UI
{
    public class StartPanel : UIPanelManager
    {
        [SerializeField] private StartPanelUI panelUI;


        public void Init()
        {
            if (panelUI == null)
                panelUI = FindObjectOfType<StartPanelUI>();
            mPanel = panelUI;
            panelUI.Init(this);
            GameManager.Instance._events.LevelLoaded.AddListener(OnNewLevel);
        }
        public void OnNewLevel()
        {
            int level = GameManager.Instance.levelManager.CurrentLevelIndex + 1;
            panelUI.SetLevel(level);
        }
        public override void OnMainButtonClick()
        {
          //  GameManager.Instance.eventManager.ClickableHit.Invoke();
            HidePanel(true);
            GameManager.Instance._events.LevelStarted.Invoke();   
        }
        public override void OnPanelHidden()
        {
            base.OnPanelHidden();
        //    GameManager.Instance.eventManager.StartPanelHidden.Invoke();
        }

    }
}