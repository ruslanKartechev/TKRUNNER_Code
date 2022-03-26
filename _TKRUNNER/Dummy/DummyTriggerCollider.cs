using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Data;
namespace TKRunner
{
    public class DummyTriggerCollider : MonoBehaviour
    {
        private DummyManager manager;
        public void Init(DummyManager _manager)
        {
            manager = _manager;
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case Tags.Bullet:
                    manager.OnBulletEnter(other.GetComponent<IBullet>());
                    break;
                case Tags.Player:
                    manager.OnPlayerContact?.Invoke();
                    break;
                case Tags.Truck:
                    manager.OnTruckTrigger?.Invoke(other.transform);
                    break;
                case Tags.Ground:
          
                    manager.OnGroundFall?.Invoke();
                    break;
                case Tags.NormalDummy:
                    manager.OnDummyTrigger?.Invoke(other.transform);
                    break;

            }

        }

    }
}