using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General;
using General.Data;
using UnityEditor;
namespace TKRunner
{
    public class DummyActivator : MonoBehaviour
    {
        [SerializeField] private List<DummyManager> _dummies = new List<DummyManager>();

        public void ActivateDummies()
        {
            foreach (DummyManager dummy in _dummies)
            {
                if (dummy != null)
                {
                    GameManager.Instance.dummyController.AddDummy(dummy);
                    dummy.gameObject.SetActive(true);
                    dummy.InitActive(GameManager.Instance.data.currentInst.levelSpline);
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

    }

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

}