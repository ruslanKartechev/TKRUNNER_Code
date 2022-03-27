using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
namespace TKRunner
{
    
    public class EnemyStopTrigger : MonoBehaviour
    {
        public Collider mColl;
        private void Start()
        {
            mColl = GetComponent<Collider>();
            if(mColl == null) { Debug.Log("no collider");return; }
            mColl.isTrigger = true;
        }
        private void OnPlayerEnter()
        {
            GameManager.Instance._events.LevelEndreached.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == Tags.Player)
                OnPlayerEnter();
        }


    }
}
