using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Commongame.UI
{
    public class UIPanel : MonoBehaviour
    {
         protected UIPanelManager panelManager;
        [SerializeField] protected Animator animator;
        [SerializeField] protected PulsingAnimationHandler pulsingAnimator;
        [SerializeField] protected GameObject Header;
        [SerializeField] protected TextMeshProUGUI headerText;
        [SerializeField] protected Button mainButton;
        [Header("Main animation states")]
        [SerializeField] protected string showStateName = "Show";
        [SerializeField] protected string hideStateName = "Hide";
        [SerializeField] protected string emptyStateName = "Idle";
        [Header("Button animation states")]
        [SerializeField] protected string btnShowStateName = "Show";
        [SerializeField] protected string btnHideStateName = "Hide";
        [SerializeField] protected string btnIdleStateName = "Idle";
        [SerializeField] protected string btnEmptyStateName = "Empty";
        [SerializeField] protected int mainAnimLayer = 1;
        [SerializeField] protected int buttonAnimLayer = 0;
        [SerializeField] public bool UseHeaderAnimator = true;




        public virtual void Init()
        {
            if(animator == null)
                animator = GetComponent<Animator>();
            if(pulsingAnimator == null)
                pulsingAnimator = GetComponent<PulsingAnimationHandler>();
            HideMainButton(false);
        }
        public void SetHeaderText(string text)
        {
            headerText.text = text;
        }
        // Panel show/hide methods
        public virtual void ShowPanel()
        {
            gameObject.SetActive(true);
            animator.Play(showStateName, mainAnimLayer, 0);
        }
        public virtual void HidePanel()
        {
            animator.Play(hideStateName, mainAnimLayer, 0);
            HideMainButton(true);

        }
        public void HidePanelImmidiate()
        {
            HideMainButton(false);
            panelManager.OnPanelHidden();
            gameObject.SetActive(false);
        }
        public void ShowPanelImmidiate()
        {
            panelManager.OnPanelShown();
            gameObject.SetActive(true);
            ShowMainButton(false);
        }
        public virtual void OnPanelShown()
        {
            animator.Play(emptyStateName, mainAnimLayer, 0);
            if (UseHeaderAnimator)
                StartHeaderAnimator();
            panelManager.OnPanelShown();
        }
        public virtual void OnPanelHidded()
        {
            animator.Play(emptyStateName, mainAnimLayer, 0);
            panelManager.OnPanelHidden();
            gameObject.SetActive(false);
        }


        public virtual void HideHeader()
        {
            if (Header == null) return;
            Header.gameObject.SetActive(false);
            headerText.gameObject.SetActive(false);
        }



        // Main Button show/hide methods
        public virtual void ShowMainButton(bool DoAnim)
        {

            if (mainButton == null)
                return;
            mainButton.gameObject.SetActive(true);
            if (DoAnim)
            {
                animator.Play(btnShowStateName, buttonAnimLayer, 0);
            }
            else
            {
                OnButtonShown();
            }
        }
        public virtual void HideMainButton(bool DoAnim)
        {
            if (mainButton == null)
                return;
            mainButton.interactable = false;
            if(DoAnim)
                animator.Play(btnHideStateName, buttonAnimLayer, 0);
            else
            {
                OnButtonHidden();
            }
        }

        public virtual void OnButtonShown()
        {
            mainButton.interactable = true;
            animator.Play(btnIdleStateName,buttonAnimLayer,0);
        }
        public virtual void OnButtonHidden()
        {
            mainButton.gameObject.SetActive(false);
        }

        // Header Animator
        public void StartHeaderAnimator(bool restart = false)
        {
            if(pulsingAnimator != null)
                pulsingAnimator.StartAnimation(restart);
        }
        public void StopHeaderAnimator()
        {
            if (pulsingAnimator != null)    
                pulsingAnimator.StopAnimation();
        }


    }
}