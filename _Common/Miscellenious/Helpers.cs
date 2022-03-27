using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Commongame
{
    public class Helpers : MonoBehaviour
    {
        public static List<int> GetRandomIndices(int start, int end, int count)
        {
            if ((end - start) < count) { Debug.Log("Wrong start/end input"); return null; }
            List<int> Pool = new List<int>();
            for (int i = start; i <= end; i++)
            {
                Pool.Add(i);
            }
            List<int> chosenInd = new List<int>();
            while (chosenInd.Count < count)
            {
                int ind = (int)Random.Range(0, Pool.Count);
                int rand = Pool[ind];
                Pool.RemoveAt(ind);
                chosenInd.Add(rand);
            }

            return chosenInd;
        }
        public static Transform FindByName(Transform parent, string name)
        {
            Transform result = null;
            foreach (Transform child in parent)
            {
                if (child.transform.name == name)
                    result = child;
            }

            return result;
        }
        public static Transform FindByTag(Transform parent, string tag)
        {
            Transform result = null;
            foreach (Transform child in parent)
            {
                if (child.transform.tag == tag)
                    result = child;
            }

            return result;
        }
        public static IEnumerator MoveToPosition(Transform target, Vector3 endPos, float speed, bool local = false)
        {

            Vector3 startPos = target.position;
            if (local == true)
                startPos = target.localPosition;
            float distance = (endPos - startPos).magnitude;
            float timeToMove = distance / speed;
            float timeElapsed = 0f;
            while (timeElapsed < timeToMove)
            {
                if (local == true)
                    target.localPosition = Vector3.Lerp(startPos, endPos, timeElapsed / timeToMove);
                else
                    target.position = Vector3.Lerp(startPos, endPos, timeElapsed / timeToMove);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            if (local == true)
                target.localPosition = endPos;
            else
                target.position = endPos;
        }
        public static IEnumerator ChangeScale(Transform target, float targetScale, float time)
        {
            float timeElapsed = 0f;
            float factor = targetScale / target.localScale.x;
            Vector3 startScale = target.localScale;
            while (timeElapsed < time)
            {
                float temp = Mathf.Lerp(1, factor, timeElapsed / time);

                target.localScale = startScale * temp;
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            target.localScale = Vector3.one * targetScale;
        }
        public static IEnumerator FromToRotationHandler(Transform target, Quaternion from, Quaternion to, float time)
        {
            float timeElapsed = 0;
            while (timeElapsed <= time)
            {

                target.rotation = Quaternion.Lerp(from, to, timeElapsed / time);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            target.rotation = to;

        }

        public static int GetRandomIndex<T>(List<T> list)
        {
            int rand = UnityEngine.Random.Range(0, list.Count);
            return rand;
        }




        public static void ClearObject(GameObject target)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(target);
            else
                Destroy(target);
#else
            Destroy(target);
#endif
        }
    }
}