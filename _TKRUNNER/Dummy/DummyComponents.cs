using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame.Sound;

namespace TKRunner
{
    public class DummyComponents : MonoBehaviour
    {
        [Header("Scripts")]
        public DummyManager _manager;
        public DummyRagdollManager _ragdoll;
        public DummySlicer _slicer;
        public DummyTriggerCollider _trigger;
        [Header("Components")]
        public Collider TriggerColl;
        public CapsuleCollider MainColl;
        public AudioSource Source;
        public SkinnedMeshRenderer Renderer;
        [Header("Sounds")]
        public SoundNames slashed;
        public SoundNames hit;
        public List<SoundNames> death;

    }
}