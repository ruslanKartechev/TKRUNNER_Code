using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Commongame;
namespace TKRunner
{
    public class ShotgunBullet : MonoBehaviour
    {
        [SerializeField] private List<Bullet> _Bullets = new List<Bullet>();

        private Coroutine _bulletsClearing;
        public void InitBullets(float speed)
        {

            foreach(Bullet b in _Bullets)
            {
                b.gameObject.SetActive(true);
                b.StartBulletForward(Random.Range(0.75f * speed, 1.25f*speed));
                b.ShowEffect();
                b.SetParent(GameManager.Instance.data.currentInst.gameObject.transform);
                GameManager.Instance._clearer.ClearWithDelay(b.transform, 
                    GameManager.Instance.data.currentInst.Data._weaponsData.BulletsClearTime);
            }
          //  _bulletsClearing = StartCoroutine(BulletsClearer());
        }

        public void GetBullets()
        {
            _Bullets = new List<Bullet>(transform.childCount);
            for (int i = 0;i<transform.childCount; i++)
            {
                Bullet b = transform.GetChild(i).gameObject.GetComponent<Bullet>();
                if (b != null)
                    _Bullets.Add(b);
            }
            _Bullets.TrimExcess();

        }
    
        //public void ClearBullets()
        //{
        //    foreach (Bullet b in _Bullets)
        //    {
        //        if(b != null)
        //            b.gameObject.SetActive(false);
        //    }
        //}
        //private IEnumerator BulletsClearer()
        //{
        //    yield return new WaitForSeconds(GameManager.Instance.data.currentInst.Data._bulletsData.BulletsClearTime);
        //    ClearBullets();

        //}

        private void OnDisable()
        {
            //if (_bulletsClearing != null) StopCoroutine(_bulletsClearing);
            //ClearBullets();
        }





    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ShotgunBullet))]
    public class ShotgunBulletEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ShotgunBullet me = target as ShotgunBullet;
            if (GUILayout.Button("GetBullets"))
                me.GetBullets();
        }

    }

#endif

}