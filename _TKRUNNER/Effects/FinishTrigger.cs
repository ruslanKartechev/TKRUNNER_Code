using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using System;
namespace TKRunner
{
    public interface ITriggerDetector
    {
        void Init(Action onEnter, string tag);
    }

    public class FinishTrigger : MonoBehaviour, ITriggerDetector
    {
        
        private string targetTag;
        private Action _onEnter;
        public bool SingleTime = true;
        public void Init(Action onEnter, string tag)
        {
            targetTag = tag;
            _onEnter = onEnter;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == targetTag)
            {
                _onEnter?.Invoke();
                if (SingleTime) _onEnter = null;
            }
        }


    }
}