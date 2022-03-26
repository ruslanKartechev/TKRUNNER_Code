using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKRunner
{

    public class DragAuraManager : MonoBehaviour
    {
        [SerializeField] private Renderer myRend;
        [SerializeField] private ParticleSystem particles_1;
        [SerializeField] private ParticleSystem particles_2;
        [SerializeField] private Color DefaultMainColor;


        public void SetSize(float size)
        {

            transform.localScale = Vector3.one * size;

        }
        public void SetParticlesSize(float size)
        {
            particles_1.transform.localScale = Vector3.one * size;
            particles_2.transform.localScale = Vector3.one * size;
        }
        public void SetColor(Color color)
        {
           if(color == Color.black)
           {
                SetColor(DefaultMainColor);
           }
            myRend.material.SetColor("_SecondColor",color);

        }

    }
}