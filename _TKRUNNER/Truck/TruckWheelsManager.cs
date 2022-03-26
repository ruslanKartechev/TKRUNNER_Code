using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKRunner
{
    public class TruckWheelsManager : MonoBehaviour
    {
        [SerializeField] private List<TruckWheel> wheels = new List<TruckWheel>();
        [SerializeField] private float PushForce = 10;
        public void Init()
        { 
            foreach(TruckWheel w in wheels)
            {
                w.Init();
            }

        }
        public void PushWheels(int num)
        {
            if (num > wheels.Count)
                num = wheels.Count;

            for(int i =0; i<num; i++)
            {
                if(wheels.Count > 0)
                {
                    wheels[0].PushAway((wheels[0].gameObject.transform.position - transform.position).normalized * PushForce);
                    wheels.Remove(wheels[0]);
                }

            }
            wheels.TrimExcess();
        }


        
    }
}