using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TKRunner
{
    public class ShotgunBullet : MonoBehaviour
    {
        [SerializeField] private List<Bullet> _Bullets = new List<Bullet>();

     
        public void InitBullets(float speed)
        {
            foreach(Bullet b in _Bullets)
            {
                b.StartBulletForward(Random.Range(0.75f * speed, 1.25f*speed));
                b.ShowEffect();
            }
        }

        public void SetBullets()
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
    }


    [CustomEditor(typeof(ShotgunBullet))]
    public class ShotgunBulletEdtor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ShotgunBullet me = (ShotgunBullet)target;

            if (GUILayout.Button("SetBullets"))
            {
                me.SetBullets();
            }

        }

    }


}