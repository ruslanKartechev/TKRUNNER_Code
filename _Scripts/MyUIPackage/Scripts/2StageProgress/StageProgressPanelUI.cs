using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using General.Data;

namespace General.UI
{
    public class StageProgressPanelUI : UIPanel
    {
        [Space(5)]
        [SerializeField] private GameObject EditorPanel;
        [Space(5)]
        [SerializeField] private StageProgressPanel panel;
        [SerializeField] private Image progressFill;
        [SerializeField] private GameObject progressBar;
        [SerializeField] protected int ProgressBarLayer;

        public void Init(StageProgressPanel _panel)
        {
            panelManager = _panel;
            panel = _panel;
            Init();
            progressFill.type = Image.Type.Filled;
            progressFill.fillMethod = Image.FillMethod.Horizontal;
            SetProgressUI(0);
        }
        public void InitStage(string header)
        {
            headerText.text = header;
            SetProgressUI(0);
        }
        public void SetProgressUI(float progress)
        {
            progressFill.fillAmount = progress;
        }
        public override void ShowPanel()
        {
            if(EditorPanel!= null)
            {
                EditorPanel.SetActive(false);
            }
            headerText.gameObject.SetActive(true);
            base.ShowPanel();
            ShowProgressBar();
        }
        public override void HidePanel()
        {
            if(EditorPanel != null)
            {
                EditorPanel.SetActive(false);
            }
            base.HidePanel();
            if(progressBar.activeInHierarchy == true)
                HidePorgressBar();
        }
        public void ShowProgressBar()
        {
            progressBar.SetActive(true);
            animator.Play("BarShow", ProgressBarLayer, 0);
        }
        public void HidePorgressBar()
        {
            animator.Play("BarHide", ProgressBarLayer, 0);
        }
        public void OnBarShown()
        {
            animator.Play("Idle", ProgressBarLayer, 0);
        }

        public void OnBarHidden()
        {
            animator.Play("Idle", ProgressBarLayer, 0);
            progressBar.SetActive(false);
        }

        public void ShowEditorPanel()
        {
            if (animator != null)
                animator.enabled = false;
            if (EditorPanel != null)
            {
                progressBar.gameObject.SetActive(false);
                headerText.gameObject.SetActive(false);
                EditorPanel.SetActive(true);
            }
            else
            {
                Debug.Log("Editor panel is not set");
            }
        }

    }
}