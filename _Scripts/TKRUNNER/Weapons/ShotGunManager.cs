using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace TKRunner {
    public class ShotGunManager : Weapon
    {
        [Space(10)]
        [SerializeField] private Transform shootingPoint;
        [SerializeField] private ShotgunBullet BulletPF;

        [SerializeField] private float BulletSpeed = 10f;
        [Header("Particle Effect")]
        [SerializeField] private ParticleSystem ShootEffect;
        //[Tooltip("Has to be an odd number")]
        //[SerializeField] private int BulletsPerShot = 10;
        //[SerializeField] private float ShotAngle = 90f;


        public void OnShootEvent()
        {
            PlayHitSound();
            ShootBullets();
            AddHitsCount();
        }
        protected override void AddHitsCount()
        {
            base.AddHitsCount();
        }


        //private List<Vector3> GetDirections(int amount, float totalAngle, Vector3 refVector)
        //{
        //    if (amount % 2 != 0)
        //        amount++;
        //    List<Vector3> result = new List<Vector3>();
        //    for(int i = 0; i < amount; i++)
        //    {
        //        float angle = Mathf.Lerp(-totalAngle / 2, totalAngle / 2, i/amount);
        //        Debug.Log("angle: " + angle);
        //        Vector3 dir = Quaternion.Euler(0, angle, 0) * refVector;
        //        result.Add(dir);
        //    }

        //    return result;
        //}

        private void ShootBullets()
        {
            ShootEffect.Stop();
            ShootEffect.Play();

            ShotgunBullet b = Instantiate(BulletPF);
            b.transform.position = shootingPoint.position;
            b.transform.rotation = shootingPoint.rotation;
            b.InitBullets(BulletSpeed);
        }
    }

    [CustomEditor(typeof(ShotGunManager))]
    public class ShotGunManagerEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ShotGunManager me = (ShotGunManager)target;
            //if (GUILayout.Button("PreSetAngles"))
            //{
            //    me.PresetShootAngles();
            //}
        }

    }
}