using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
namespace TKRunner {
    public class WeaponCollisionManager : MonoBehaviour
    {
        [SerializeField] private float PushForce = 10;

        public float GetPushForce()
        {
            return PushForce;
        }

        public Vector3 GetPushSource()
        {
            return GameManager.Instance.data.Player.gameObject.transform.position;
        }
    }
}