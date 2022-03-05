using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using General.Data;
using Dreamteck.Splines;
namespace TKRunner {

    [RequireComponent(typeof(Collider))]
    public class SpawnCheckpoint : MonoBehaviour
    {

        [SerializeField] private DummyActivator activator;
     //   public SplineProjector spawnPoint;
      //  public float spawnInterval = 5;
       // [SerializeField] private bool accelerateOnStart = false; 
       // [SerializeField] private float Radius = 1f;

        private void OnContact()
        {
            //if(spawnPoint == null)
            //{
            //    Debug.Log("Spawn point not assigned");
            //    return;
            //}
            //GameManager.Instance.dummySpawner.SpawnOnCP(spawnPoint,spawnInterval,accelerateOnStart);
            if (activator == null)
                activator = GetComponent<DummyActivator>();
            activator?.ActivateDummies();


        }


        //private float GetSpawnRad()
        //{
        //    return spawnPoint.spline.CalculateLength()*spawnInterval*0.01f;
        //}

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.tag == Tags.Player)
            {
                OnContact();
            }
        }
        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.DrawWireSphere(spawnPoint.transform.position, GetSpawnRad());
        //}
    }
}