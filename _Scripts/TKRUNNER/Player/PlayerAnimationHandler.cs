using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TKRunner
{
    public class PlayerAnimationHandler : MonoBehaviour
    {
        [SerializeField] private PlayerManager manager;

        public void OnThrowAnimEvent()
        {
            manager.OnThrowEvent();
        }
    }
}