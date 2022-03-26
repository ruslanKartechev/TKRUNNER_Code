using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TKRunner
{
    public class PlayerRagdollManager : RagdollManager
    {


    }


#if UNITY_EDITOR

    [CustomEditor(typeof(PlayerRagdollManager))]
    public class PlayerRagdollManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PlayerRagdollManager me = (PlayerRagdollManager)target;
            if (GUILayout.Button("SetPassive"))
            {
                me.GetRagdoll();
                me.SetPassive();
            }
        }

    }

#endif
}