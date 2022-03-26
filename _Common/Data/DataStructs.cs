using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commongame.Data
{
    public struct Runners
    {
        public const string Player = "Player";
        public const string Dummy = "Dummy";
    }

    public struct CheckpointNames
    {

    }
    public struct Tags
    {
        public const string OuterCollider = "OuterCollider";
        public const string NormalDummy = "NormalDummy";
        public const string GuardedDummy = "GuardedDummy";
        public const string Player = "Player";
        public const string Ground = "Ground";
        
        public const string Obstacle = "Obstacle";
        public const string Wall = "Wall";
        public const string Box = "Box";

        public const string Portal = "Portal";
        public const string LazerBeam = "Lazer";


        public const string LevelEnd = "LevelEnd";
        public const string Ragdoll = "Ragdoll";
        // Weapons
        public const string Sword = "Sword";
        public const string Bat = "Bat";
        public const string Axe = "Axe";
        public const string Bullet = "Bullet";

        public const string Truck = "Truck";


    }
    public struct DummyTypes
    {
        public const string Normal = "Normal";
        public const string Guarded = "Guarded";
    }

    public struct AnimNames
    {
        public const string IdleRunTree = "IdleRunTree";
        public const string FallLayTree = "FallLayTree";
        public const string StandUpAndRunTree = "StandUpAndRunTree";

        public const string DummyIdle = "Idle";
        public const string DummyRun = "Run";

        public const string DummyHold = "Hold";
        public const string LongFall = "LongFall";
        public const string ShortFall = "ShortFall";
        public const string Laying = "Laying";
        public const string DummyStandUp = "StandUp";
        public const string DummyTruckIdle = "TruckIdle";
        public const string DummyTruckJump = "TruckJump";
        public const string DummyWait = "Wait";
        public const string ZombieBiting = "ZombieBiting";
        public const string DoublePound = "DoublePound";
        public const string LookAround = "LookingAround";
        public const string PortalJump = "PortalJump";


        public const string DummyDead = "Dead";

        public const string Win = "Win";
        public const string Defeat = "Defeat";
        // Magic
        public const string MagicTree = "MagicTree";
        public const string MagicIdle = "MagicIdle";
        public const string MagicThrow = "MagicThrow";
        // Weapons
        public const string WeaponActive = "Active";
        public const string WeaponIdle = "Idle";
        public const string WeaponThrown = "Thrown";

        // Player
        public const string ShowOff = "ShowOff";
        public const string DummyJump = "Jump";
        public const string DummyLaying = "Lay";
        public const string ForwardRun = "ForwardRun";
        public const string Roll = "Roll";


    }
    public enum SoundNames
    {
        Magic,
        Scream,
        WallHit,
        AxeSwing,
        SwordSwing,
        HammerSwing,
        BatSwing,
        AxeHit,
        SwordHit,
        HammerHit,
        BatHit,
        ToolBreak,
        Lazer,
        Portal,
        DummyCollision,
        Win,
        GameOver,
        Finish,
        ButtonClick,
        ShotGunFire,
        BoxBreak,

        Music_1
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

    public struct Sounds
    {
        public const string Magic = "Magic";
        public const string Scream = "Scream";
        public const string WallHit = "WallHit";
        // weapons
        public const string AxeSwing = "AxeSwing";
        public const string SwordSwing = "SwordSwing";
        public const string HammerSwing = "HammerSwing";
        public const string BatSwing = "BatSwing";
        public const string AxeHit = "AxeHit";
        public const string SwordHit = "SwordHit";
        public const string HammerHit = "HammerHit";
        public const string BatHit = "BatHit";
        public const string WeaponBreak = "ToolBreak";
        public const string Portal = "Portal";
        public const string Lazer = "Lazer";
        //
        public const string DummyCollision = "DummyCollision";
        public const string Win = "Win";
        public const string Loose = "Loose";
        public const string Finish = "Finish";

        //
        public const string Music_1 = "Music_1";
        //
        public const string ButtonClick = "ButtonClick";


    }
    public struct SourceByName
    {
        public const string MusicSource = "Music";
        public const string EffectsSource = "Effects";
    }



}