

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TKRunner
{
    [CustomEditor(typeof(ShotGunManager))]
    public class ShotGunManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ShotGunManager me = (ShotGunManager)target;
        }

    }
}
#endif