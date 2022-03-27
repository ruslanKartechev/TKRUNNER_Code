using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;

namespace TKRunner
{
    public class DragVelocityCalculator : MonoBehaviour
    {
        [SerializeField] private Rigidbody KineMaticRB;
        private Coroutine _calculator;
        public void SetTarget(Rigidbody rb)
        {
            KineMaticRB = rb;
        }
        public void StartCalculator()
        {
            if(KineMaticRB == null) { Debug.Log("kinematic rb not set"); return; }
            StopStopCalculator();
            _calculator = StartCoroutine(VelocityCalculator());
        }
        public void StopStopCalculator()
        {
            if (_calculator != null) StopCoroutine(_calculator);
            OutputDragVel(Vector3.zero);
        }

        private IEnumerator VelocityCalculator()
        {
            Vector3 oldPos = KineMaticRB.position;
            Vector3 newPos = KineMaticRB.position;
            while (KineMaticRB != null)
            { 
                newPos = KineMaticRB.position;
                OutputDragVel((newPos - oldPos) / Time.deltaTime) ;
                oldPos = newPos;
                yield return new WaitForFixedUpdate();
            }


        }

        public void OutputDragVel(Vector3 vel)
        {
            GameManager.Instance._data.CurrentDragVelocity = vel;
        }
    }
}