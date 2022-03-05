﻿
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    [CustomEditor(typeof(LevelManager))]
    public class InspectorForLM: Editor
    {
        private ReorderableList listLvl;
        private LevelManager _levelManager;
        private SerializedProperty _editorMode;

        private void Awake()
        {
            _levelManager = target as LevelManager;
        }
        private void OnEnable()
        {
            _editorMode = serializedObject.FindProperty("editorMode");
            listLvl = new ReorderableList(serializedObject, serializedObject.FindProperty("Levels"), true, true, true, true);
            listLvl.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = listLvl.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.IntField(new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight), index + 1);

                if (GUI.Button(new Rect(rect.x + 36, rect.y, 50, EditorGUIUtility.singleLineHeight), new GUIContent("Select")))
                {
                    serializedObject.FindProperty("CurrentLevelIndex").intValue = index;
                    serializedObject.ApplyModifiedProperties();
                    _levelManager.InitLevel(index);
                }

                EditorGUI.PropertyField(
                    new Rect(rect.x + 90, rect.y, 200, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("lvlPF"), GUIContent.none);


                if (GUI.Button(new Rect(rect.x + 300, rect.y, 50, EditorGUIUtility.singleLineHeight), new GUIContent("Clear")))
                {
                    _levelManager.ClearListAtIndex(index);
                }

                if (GUI.Button(new Rect(rect.x + 347, rect.y, 50, EditorGUIUtility.singleLineHeight), new GUIContent("Delete")))
                {
                    _levelManager.Levels.RemoveAt(index);
                }
            };

            listLvl.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Levels");
            };
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _editorMode.boolValue = GUILayout.Toggle(_editorMode.boolValue, new GUIContent("Editor Mode"), GUILayout.Width(100), GUILayout.Height(20));
            serializedObject.ApplyModifiedProperties();
            _levelManager.editorMode = _editorMode.boolValue;
            if (_editorMode.boolValue == true) DrawSelectedLevel();

            serializedObject.Update();
            listLvl.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

           
            

            if (GUILayout.Button("Clear Player Prefs", GUILayout.Width(200), GUILayout.Height(20)))
                PlayerPrefs.DeleteAll();
        }

        private void DrawSelectedLevel()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            int index = EditorGUILayout.IntField("Current Level", _levelManager.CurrentLevelIndex + 1);
            if (EditorGUI.EndChangeCheck())
            {
                _levelManager.InitLevel(index - 1);
            }

            if (GUILayout.Button("<<", GUILayout.Width(30), GUILayout.Height(20)))
            {
                _levelManager.PrevLevel();
            }
            if (GUILayout.Button(">>", GUILayout.Width(30), GUILayout.Height(20)))
            {
                _levelManager.NextLevel();
            }
     
            EditorGUILayout.EndHorizontal();
        }

      








    }


# endif