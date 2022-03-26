using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TKRunner
{
# if UNITY_EDITOR
    [CustomEditor(typeof(DummyRagdollManager))]
    public class DummyRagdollManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DummyRagdollManager me = (DummyRagdollManager)target;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Get Ragdoll"))
            {
                me.GetRagdoll();
            }
            if(GUILayout.Button("Set Passive"))
            {
                me.SetPassive();
            }
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Add Force"))
            {
                me.TestPush();
            }
        }

    }
#endif
}