using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;

namespace Commongame.UI
{
    public class StageProgressPanel : UIPanelManager
    {
        [SerializeField] private StageProgressPanelUI panelUI;
        private bool IsEditor;
        public void Init()
        {
            mPanel = panelUI;
            panelUI.Init(this);
            IsEditor = GameManager.Instance.data.EditorUIMode;

        }

        public override void ShowPanel(bool showButton = true)
        {
            if(IsEditor == false)
            {
                base.ShowPanel(showButton);
            }
            else
            {
              //  panelUI.gameObject.SetActive(true);
               // panelUI.ShowEditorPanel();
            }

        }

        public override void SwitchHeader(int dir)
        {
            base.SwitchHeader(dir);
            panelUI.StartHeaderAnimator();
        }


    }
}