using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;

namespace General.Data {
    public class DataLoader : MonoBehaviour
    {
        public void StartLoading()
        {
            StartCoroutine(Load());
        }


        private IEnumerator Load()
        {
           // Debug.Log("Started Loading");
            yield return new WaitForSeconds(1f);
         //   Debug.Log("Finished Loading");
            GameManager.Instance.eventManager.DataLoaded.Invoke();
        }
    }
}