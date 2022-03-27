using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commongame;
using Commongame.Data;
using UnityEditor;
namespace TKRunner
{

    [DefaultExecutionOrder(999)]
    public class DummyActivator : MonoBehaviour, ISpawner
    {
        [SerializeField] private ActivatorModes _mode;

        [SerializeField] private List<DummyManager> _dummies = new List<DummyManager>();

        private void Awake()
        {
            GameManager.Instance._events.LevelLoaded.AddListener(OnNewLevel);
        }
        private void OnNewLevel()
        {
            GetDummiesEditor();
            switch (_mode)
            {
                case ActivatorModes.Default:
                    foreach (DummyManager dm in _dummies)
                    {
                        if (dm != null)
                            dm.gameObject.SetActive(false);
                    }
                    break;
                case ActivatorModes.Forward:
                    foreach (DummyManager dm in _dummies)
                    {
                        if (dm != null)
                        {
                            dm.InitWaiting();
                        }
                    }
                    break;
            }
        }
        public void Spawn()
        {
            switch (_mode)
            {
                case ActivatorModes.Default:
                    DefaultActivation();
                    break;
                case ActivatorModes.Forward:
                    ForwardActivation();
                    break;
            }
        }

        private void DefaultActivation()
        {
            foreach (DummyManager dummy in _dummies)
            {
                if (dummy != null)
                {
                  
                    dummy.gameObject.SetActive(true);
                    dummy.SpawnDefault();
                }

            }
        }

        private void ForwardActivation()
        {
            foreach (DummyManager dummy in _dummies)
            {
                if (dummy != null)
                {
                    
                    dummy.gameObject.SetActive(true);
                    dummy.StartFromWaiting();
                }

            }
        }

        public void GetDummiesEditor()
        {
            _dummies = new List<DummyManager>();
            for(int i=0; i<transform.parent.childCount; i++)
            {
                DummyManager dum = transform.parent.GetChild(i).GetComponent<DummyManager>();
                if (dum != null)
                {
                    _dummies.Add(dum);
                }
            }
        }

        public void SetSpawnCount(int count)
        {
            //
        }
    }

# if UNITY_EDITOR

    [CustomEditor(typeof(DummyActivator))]
    public class DummyActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DummyActivator me = (DummyActivator)target;
            if (GUILayout.Button("GetDummies"))
            {
                me.GetDummiesEditor();
            }
        }
    }
#endif


}