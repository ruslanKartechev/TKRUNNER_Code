using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TKRunner
{
   
    //public interface IWeapon
    //{
    //    float GetPushForce();
    //    Vector3 GetPushSource();
    //}
    //public interface IPushable
    //{
    //    void PushAway(Vector3 origin, float force);
    //}

    public interface IBreakable
    {

        void Break();
    }
    public interface IWeaponTarget
    {
        void Slash(Plane plane);
        void KillAndPush(Vector3 force);
        void PushAway(Vector3 origin, float force);
        Transform GetTransform();
    }


    public interface ISclicing
    {
        Plane GetSlicePlane();
    }
}