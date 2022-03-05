using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
namespace TKRunner
{
    public class LazerSpinnerManager : MonoBehaviour
    {

        private List<LazerBeam> beams;
        private Coroutine rotation;
        private LazerSpinnerController boss;

        public void Init(LazerSpinnerController _boss, List<LazerBeam> _beams)
        {
            beams = _beams;
            boss = _boss;
            foreach(LazerBeam b in _beams)
            {
                b.Init();
            }

        }

        public void StartLazers()
        {
            foreach(LazerBeam b in beams)
                b.Activate();
        }
        public void StopLazers()
        {
            foreach (LazerBeam b in beams)
                b.DeActivate();
        }
        public void StartMovement()
        {
            StopMovement();
            MovingHandler();
        }
        public void StopMovement()
        {
            if (rotation != null) StopCoroutine(rotation);
        }

        private void MovingHandler()
        {
            StopMovement();
            rotation = StartCoroutine(RotationHandler());

        }
       private IEnumerator RotationHandler()
        {
            while (true)
            {
                SetAngle(boss.Data.RotSpeed * Time.deltaTime);

                yield return null;
            }
        }

        private void SetAngle(float y)
        {
            transform.eulerAngles += new Vector3(0, y, 0);
        }

        private void OnDisable()
        {
            if (rotation != null)
                StopCoroutine(rotation);
        }
    }
}