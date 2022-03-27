using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
namespace TKRunner
{
    public class HandController : MonoBehaviour
    {
        
        [SerializeField] private HandManager _manager;
        private Coroutine inputCheck;


        private void Start()
        {
            _manager.HideHand();
            if (GameManager.Instance._data.EditorUIMode)
            {
                StartInputCheck();
            }
        }
        public void StopInputCheck()
        {
            if (inputCheck != null) StopCoroutine(inputCheck);
        }
        public void StartInputCheck()
        {
            StopInputCheck();
            inputCheck = StartCoroutine(InputChecking());
        }
        private void OnClick()
        {
            _manager.StartPointerFollow();
            _manager.Show();
        }
        private void OnRelease()
        {
            _manager.Hide();
        }


        private IEnumerator InputChecking()
        {
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnClick();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    OnRelease();
                }
                yield return null;
            }
        }

    }
}