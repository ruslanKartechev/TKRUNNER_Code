using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Commongame.UI
{
    public class LevelCompletePanelUI : UIPanel
    {
        private LevelCompletePanel panel;
        [Space(5)]
        [SerializeField] TextMeshProUGUI ButtonText;
        public void Init(LevelCompletePanel _panel)
        {
            panelManager = _panel;
            panel = _panel;
            Init();
        }
        public void SetLevel(int CurrentLevel)
        {
            string leveltext = "Level " + CurrentLevel.ToString() + " Completed";
            SetHeaderText(leveltext);
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            mainButton.gameObject.SetActive(true);
            mainButton.interactable = true;
            mainButton.onClick.AddListener(panel.OnMainButtonClick);
            mainButton.onClick.RemoveListener(panel.OnRetryButtonClick);
            mainButton.interactable = true;
            ButtonText.text = "Next";
            animator.Play("ButtonIdle", buttonAnimLayer, 0);
        }

        public override void HidePanel()
        {
            base.HidePanel();
        }

        public void ShowRetryButton()
        {
            if (mainButton == null) return;
            mainButton.gameObject.SetActive(true);
            mainButton.interactable = true;

            mainButton.onClick.RemoveListener(panel.OnMainButtonClick);
            mainButton.onClick.AddListener(panel.OnRetryButtonClick);
            ButtonText.text = "Retry";
            animator.Play("ButtonIdle", buttonAnimLayer, 0);
        }

    }
}