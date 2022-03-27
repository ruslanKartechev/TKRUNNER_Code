using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Sound;
using Commongame;

namespace TKRunner
{
    
    public class DummySoundManager
    {

        public DummySoundManager() { }
        public DummySoundManager(DummyComponents componenets)
        {
            _componenets = componenets;
        }
        public DummyComponents _componenets;

        public void OnSlashed()
        {
            GameManager.Instance._sounds.PlaySingleTime(_componenets.slashed.ToString()); 
        }
        public void OnDeath()
        {
            int ind = Helpers.GetRandomIndex<SoundNames>(_componenets.death);
            GameManager.Instance._sounds.PlaySingleTime(_componenets.death[ind].ToString());
        }
        public void OnHit()
        {
            GameManager.Instance._sounds.PlaySingleTime(_componenets.hit.ToString());
        }





    }
}
