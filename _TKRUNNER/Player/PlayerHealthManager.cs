using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
namespace TKRunner
{
    public class PlayerHealthManager : MonoBehaviour
    {

        [SerializeField] private float colorChangeTime;
        [Space(5)]
        [SerializeField] private SkinnedMeshRenderer rend;
        [SerializeField] private Color damagedColor;

        private PlayerController controller;
        private int hitsToDie;
        private int hitsTaken = 0;
        private bool isDead = false;
        private Color startColor;

        private Coroutine colorChange;
        private float countDown = 0f;
        private int startHitsMax;
        private Coroutine healthAdding;
        public void Init(PlayerController _cont, PlayerHealthData data)
        {
            startColor = rend.material.color;
            controller = _cont;
            int health = data.StartHealth;
            if(health < 1)
            {
                health = 1;
            }
            hitsToDie = health;
            startHitsMax = health;

            if (data.AddHealth)
            {
                if (healthAdding != null) StopCoroutine(healthAdding);
                healthAdding = StartCoroutine(LifeAddCountdown(data.HealthAddDelay));
            }

        }




        public bool DoDie()
        {
            hitsTaken++;
            if (hitsTaken >= hitsToDie && isDead == false)
            {
                if (healthAdding != null) StopCoroutine(healthAdding);
                Die();
                return true;
            }
            else
            {
                TakeHit();
                return false;
            }
        }

        public void TakeHit()
        {
            if (colorChange != null)
            {
                StopCoroutine(colorChange);
                SetMatColor(startColor);
            }
            colorChange = StartCoroutine(ColorFlick());
        }


        private IEnumerator ColorFlick()
        {
            yield return null;
            SetMatColor(damagedColor);
            yield return new WaitForSeconds(colorChangeTime);
            SetMatColor(startColor);
        }
        private IEnumerator LifeAddCountdown(float time)
        {
            while (true)
            {

                countDown += Time.deltaTime;
                if(countDown >= time)
                {
                    AddHitsMax();
                    countDown = 0f;
                }


                yield return null;
            }
        }

        private void AddHitsMax()
        {
            if (hitsToDie < startHitsMax + 1)
            {
                hitsToDie++;
            }
        }


        private void SetMatColor(Color color)
        {
            rend.material.color = color;
        }
        public void Die()
        {
            isDead = true;
            if (colorChange != null)
            {
                StopCoroutine(colorChange);
                SetMatColor(startColor);
            }

        }


    }
}