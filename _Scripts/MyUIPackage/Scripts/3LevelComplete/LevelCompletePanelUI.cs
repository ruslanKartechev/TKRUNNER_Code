using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace General.UI
{
    public class LevelCompletePanelUI : UIPanel
    {
        private LevelCompletePanel panel;
        //[Space(5)]
        //[SerializeField] private GameObject progressBar;
        //[SerializeField] private Image progressFill;
        //[Space(5)]
        //[SerializeField]  private string barShowName;
        //[SerializeField]  private string barHideName;
        //[SerializeField] private int progressBarLayer = 2;
        [Space(5)]
        [SerializeField] TextMeshProUGUI barText;
        private float currentProgress;
        public void Init(LevelCompletePanel _panel)
        {
            panelManager = _panel;
            panel = _panel;
            Init();
            mainButton.onClick.AddListener(_panel.OnMainButtonClick);
            //progressFill.type = Image.Type.Filled;
            //progressFill.fillMethod = Image.FillMethod.Horizontal;
        }
        public void SetLevel(int CurrentLevel)
        {
            string leveltext = "Level " + CurrentLevel.ToString() + " Completed";
            SetHeaderText(leveltext);
            SetProgressBarText(0);
        }


        public void SetCP_1_Prog(float progress)
        {
            currentProgress = progress;
        }
        public void SetProgressBarText(float progress)
        {
            if(barText != null)
                barText.text = Mathf.Round(progress*100) + "/100";

        }
        public override void ShowPanel()
        {
            base.ShowPanel();
            ShowMainButton(true);
        }
        public override void HidePanel()
        {
            base.HidePanel();

        }

        private IEnumerator FillingProgress()
        {
            float time = 1f;
            float timeElapsed = 0f;
            while(timeElapsed <= time)
            {
               float prog = Mathf.Lerp(0, currentProgress, timeElapsed / time);
                SetProgressBarText(prog);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            SetProgressBarText(currentProgress);
            ShowMainButton(true);
        }

    }
}