using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKRunner
{
    public class Lantern : MonoBehaviour
    {
        [SerializeField] private Animator anim;

        public void Move()
        {
            anim.Play("Active",0, UnityEngine.Random.Range(0f,0.15f));
        }
        public void Stop()
        {
            anim.Play("Idle");
        }
    }
}