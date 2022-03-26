using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace TKRunner
{
    public class TruckDummyController : MonoBehaviour
    {
        [SerializeField] private List<DummyManager> leftSide = new List<DummyManager>();
        [SerializeField] private List<DummyManager> rightSide = new List<DummyManager>();
        private List<DummyManager> allDummies = new List<DummyManager>();
        public int TotalDummyCount { get { return allDummies.Count; } }

        [SerializeField] private float PushForce;
        public void Init(SplineFollower follower)
        {
            foreach(DummyManager dummy in leftSide)
            {
                dummy?.InitTrucker(follower.spline,false);
            }
            foreach (DummyManager dummy in rightSide)
            {
                dummy?.InitTrucker(follower.spline, true);
            }
            allDummies.AddRange(leftSide);
            allDummies.AddRange(rightSide);
        }
        public void DeployAll(float currentSpeed)
        {
            DeployLeft(currentSpeed);
            DeployRight(currentSpeed);

        }
        public void DeployLeft(float currentSpeed)
        {
            foreach (DummyManager dummy in leftSide)
            {
                dummy?.JumpFromTruck(currentSpeed, false);
            }
        }
        public void DeployRight(float currentSpeed)
        {
            foreach (DummyManager dummy in rightSide)
            {
                dummy?.JumpFromTruck(currentSpeed, true);
            }
        }


        
        public void PushDummy(int num)
        {
            allDummies.TrimExcess();
            if (num > allDummies.Count)
                num = allDummies.Count;

            for (int i = 0; i < num; i++)
            {
                if(allDummies.Count > 0)
                {
                    allDummies[0].PushFromTruck(transform.position, PushForce);
                    allDummies.Remove(allDummies[0]);

                }

            }
          
         
        }

        
    }
}