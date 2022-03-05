using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
namespace General.UI
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


        //public void SetFinalProgress(float progress)
        //{
        //    panelUI.SetCP_1_Prog(progress);
        //}


        //public void InitCheckpoint_PB_1(float progress)
        //{
        //    panelUI.gameObject.SetActive(true);
        //    panelUI.SetCP_1_Prog(progress);
        //    panelUI.ShowCP_1_PB(false);

        //}
        //public void InitEndLevelPB(float progress)
        //{

        //}



        public override void OnMainButtonClick()
        {
            HidePanel(false);
            GameManager.Instance.eventManager.NextLevelCalled.Invoke();
        }
    }
}