using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Commongame.Sound
{
    public enum SoundNames
    {
        Magic,
        Scream_1,
        Scream_2,
        Scream_3,

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
        LevelStart,
        Win,
        GameOver,
        Finish,
        ButtonClick,
        ShotGunFire,
        BoxBreak,



        Music_1
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