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
        public void Init(PlayerController _cont, int health)
        {
            startColor = rend.material.color;
            controller = _cont;
            hitsToDie = health;
        }

        public bool DoDie()
        {
            hitsTaken++;
            if (hitsTaken >= hitsToDie && isDead == false)
            {
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