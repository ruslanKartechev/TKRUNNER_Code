using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TKRunner
{
    public class LanternsManager : MonoBehaviour
    {
        [SerializeField] private List<Lantern> _lanterns = new List<Lantern>();

        public void StartLanters()
        {
            foreach (Lantern l in _lanterns)
                l.Move();
        }
        public void StopLanterns()
        {
            foreach (Lantern l in _lanterns)
                l.Stop();
        }
        
    }
}