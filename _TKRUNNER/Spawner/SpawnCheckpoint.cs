using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;
using Dreamteck.Splines;
namespace TKRunner {

    [RequireComponent(typeof(Collider))]
    public class SpawnCheckpoint : MonoBehaviour
    {

        [SerializeField] private ISpawner activator;

        private void Start()
        {
            activator = GetComponent<ISpawner>();
        }
        private void OnContact()
        {

            if (activator != null)
                activator?.Spawn();


        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == Tags.Player)
            {
                OnContact();
            }
        }

    }
}