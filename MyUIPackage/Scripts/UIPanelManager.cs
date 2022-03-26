using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Commongame.UI
{

    public class UIPanelManager : MonoBehaviour
    {

        protected UIPanel mPanel;
        protected List<string> mHeaders = new List<string>();

        protected int currentHeader = 0;
        public virtual void OnMainButtonClick()
        {

        }
        public virtual void ShowPanel(bool showButton = true)
        {
            if (!showButton)
            {
                mPanel.HideMainButton(false);
            }
            mPanel.ShowPanel();
            if (showButton)
            {
                mPanel.ShowMainButton(true);
            }
        }
        public virtual void HidePanel(bool immidiate)
        {
            if (mPanel.gameObject.activeInHierarchy == false)
                return;
            if (!immidiate)
                mPanel.HidePanel();
            else
                mPanel.HidePanelImmidiate();
        }
        public virtual void SwitchHeader(int dir)
        { 
            currentHeader += dir;
            if (currentHeader >= mHeaders.Count || currentHeader < 0)
                return;
            string newHeader = mHeaders[currentHeader];
            mPanel.SetHeaderText(newHeader);
        }
        public virtual void ShowMainButton(bool animate)
        {
            mPanel.ShowMainButton(animate);
        }
        public virtual void HideMainButton(bool animate) {
            mPanel.HideMainButton(animate);
        }



        public virtual void OnPanelHidden()
        {
        }
        public virtual void OnPanelShown()
        {
        }

    }
}