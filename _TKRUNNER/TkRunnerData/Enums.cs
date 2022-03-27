using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TKRunner
{
    public enum DummyStates
    {
        Idle, Waiting, Run, Drag, Thrown, Standup, Truck, Dead
    }


    public enum WeaponType
    {
        Default, Hammer, Shotgun, Sword, Axe, Bat
    }
    public enum ActivatorModes
    {
        Default, Forward, Hunter
    }

    public enum SpawnMode
    {
        OneByOne,
        AllInOne
    }
}