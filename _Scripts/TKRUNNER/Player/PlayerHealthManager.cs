using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
namespace TKRunner
{
    public class PlayerHealthManager : MonoBehaviour
    {
        [SerializeField] private int hitsToDie;
        [SerializeField] private float colorChangeTime;
        [Space(5)]
        [SerializeField] private SkinnedMeshRenderer rend;
        [SerializeField] private Color damagedColor;

        private PlayerController controller;
        private int hitsTaken = 0;
        private bool isDead = false;
        private Color startColor;

        private Coroutine colorChange;
        public void Init(PlayerController _cont)
        {
            startColor = rend.material.color;
            controller = _cont;
        }
        public void TakeHit()
        {
            hitsTaken++;
            if(hitsTaken >= hitsToDie && isDead == false)
            {
                Die();
                return;
            }
            GameManager.Instance.eventManager.Impact.Invoke();
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
            controller.OnDeath();
            GameManager.Instance.eventManager.PlayerLose.Invoke();
        }


    }
}