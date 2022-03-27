using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Commongame;
namespace TKRunner {
    public class ShotGunManager : Weapon
    {
        [Space(10)]
        [SerializeField] private Transform shootingPoint;
        [SerializeField] private ShotgunBullet BulletPF;
        [Header("Particle Effect")]
        [SerializeField] private ParticleSystem ShootEffect;
        [Header("CamShake and Haptic")]
        [SerializeField] private bool ShakeOnFire = true;

        private List<ShotgunBullet> preInstBullets;

        private void Start()
        {
            PreInstBullets();
        }
        private void PreInstBullets()
        {
            preInstBullets = new List<ShotgunBullet>((int)HitsToBreak);
            for(int i = 0; i< (int)HitsToBreak; i++)
            {
                ShotgunBullet temp = Instantiate(BulletPF);
                temp.gameObject.SetActive(false);
                temp.gameObject.name += " " + i.ToString();
                temp.gameObject.transform.parent = transform;
                preInstBullets.Add(temp);

            }
        }

        public void OnShootEvent()
        {
            PlayHitSound();
            ShootBullets();

        }
        protected override void AddHitsCount()
        {
            base.AddHitsCount();
        }

        private void ShootBullets()
        {
            ShootEffect.Stop();
            ShootEffect.Play();
            ShotgunBullet b = null;
            if (hitCount < preInstBullets.Count && preInstBullets[hitCount] != null)
                 b = preInstBullets[hitCount];//Instantiate(BulletPF);
            else
                 b = Instantiate(BulletPF);
            b.transform.parent = transform.parent;
            b.gameObject.SetActive(true);
            b.transform.position = shootingPoint.position;
            b.transform.rotation = shootingPoint.rotation;
  
            AddHitsCount();
            b.InitBullets(GameManager.Instance._data.currentInst.Data._weaponsData.BulletsSpeed);

            if (ShakeOnFire)
                GameManager.Instance._events.Impact.Invoke();
     
        }
    }


}