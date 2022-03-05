using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;

namespace TKRunner
{

    public class DummyPortal : MonoBehaviour
    {
        [SerializeField] private DummyPortal OutPortal;
        [SerializeField] private PortalData settings = new PortalData();
        [SerializeField] private ParticleSystem _particles;

        public PortalData GetOutPortalData()
        {
            if(OutPortal == null)
            {
                Debug.Log("Out portal not assinged");
                return null;
            }
            settings.outPosition = OutPortal.transform.position;
            settings.outForward = OutPortal.transform.forward;
            settings.inPosition = transform.position;
            return settings;

        }

        public void ShowEffect()
        {
            if (_particles == null)
                return;
            _particles.Play();
        }


    }
}