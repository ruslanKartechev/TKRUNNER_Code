using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
using Commongame;
namespace TKRunner
{
    public class TruckSpawner : MonoBehaviour
    {
        [SerializeField] private TruckManager truck;

        public void SpawnTruck()
        {
            truck.gameObject.SetActive(true);
            truck.Init(GameManager.Instance._data.Player);
        }


        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.tag == Tags.Player)
            {
                SpawnTruck();
            }
        }
    }
}