using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;

namespace Commongame.Data {
    public class DataLoader : MonoBehaviour
    {
        public void StartLoading()
        {
            StartCoroutine(Load());
        }


        private IEnumerator Load()
        {
            // Debug.Log("Started Loading");
            //yield return new WaitForSeconds(1f);
            //   Debug.Log("Finished Loading");
            yield return null;
            GameManager.Instance.eventManager.DataLoaded.Invoke();
        }
    }
}