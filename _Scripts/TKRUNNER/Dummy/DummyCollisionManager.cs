using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using General.Data;

namespace TKRunner
{
    public class DummyCollisionManager : MonoBehaviour
    {
        private DummyManager manager;
       public void Init(DummyManager _manager)
        {
            manager = _manager;
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    switch (collision.collider.tag)
        //    {
        //        case Tags.NormalDummy:
        //            manager.SetFollowing(false);
        //            manager.OnDragStop();
        //            break;
        //        case Tags.GuardedDummy:
        //            manager.SetFollowing(false);
        //            manager.OnDragStop();
        //            break;
        //        case Tags.Obstacle:
        //            manager.SetFollowing(false);
        //            manager.OnDragStop();
        //            break;
        //        case Tags.Ground:
        //            manager.SetGround(true);
        //            break;
        //    }
        //}

        //private void OnCollisionExit(Collision collision)
        //{
        //    switch (collision.collider.tag)
        //    {
        //        case Tags.Ground:
        //            manager.SetGround(false);
        //            break;
        //    }
        //}


    }
}

