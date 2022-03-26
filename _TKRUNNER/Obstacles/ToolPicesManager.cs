using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Commongame;
namespace TKRunner
{
    public enum ColliderType
    {
        Sphere, Box
    }
    public class ToolPicesManager : MonoBehaviour, IBreakable
    {
        [SerializeField] private List<Collider> pieces = new List<Collider>();
        [SerializeField] private int PushForce = 1;
        [SerializeField] private ColliderType collType;
        public void InitPieces()
        {
            if (pieces == null)
                return;
            foreach(Collider c in pieces)
            {
                if(c != null)
                {
                    c.gameObject.SetActive(true);
                    c.enabled = true;
                    c.isTrigger = false;
                    c.attachedRigidbody.isKinematic = false;
                    c.attachedRigidbody.AddForce(UnityEngine.Random.onUnitSphere * PushForce, ForceMode.Impulse);
                   

                    c.transform.parent = GameManager.Instance.data.currentInst.transform;
                    GameManager.Instance._clearer.ClearWithDelay(c.transform, GameManager.Instance._clearer.DefaultClearDelay);
                }
            }
      
        }



        public void GetPiecesRBs()
        {
            pieces = new List<Collider>();
            for (int i = 0; i < transform.childCount; i++)
            {
                
                Collider c = transform.GetChild(i).GetComponent<Collider>();
                if (c == null)
                {
                    switch (collType)
                    {
                        case ColliderType.Sphere:
                            c = transform.GetChild(i).gameObject.AddComponent<SphereCollider>();
                            break;
                        case ColliderType.Box:
                            c = transform.GetChild(i).gameObject.AddComponent<BoxCollider>();
                            break;
                    }
      
                }
                pieces.Add(c);
                c.enabled = false;
                Rigidbody rb = c.gameObject.GetComponent<Rigidbody>();
                if (rb == null)
                    rb = c.gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;

            }

        }

        public void Break()
        {
            gameObject.transform.parent = GameManager.Instance.data.currentInst.gameObject.transform ;
            InitPieces();
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ToolPicesManager))]
    public class ToolPicesManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ToolPicesManager me = (ToolPicesManager)target;
            if(GUILayout.Button("Get RigidBodies"))
            {
                me.GetPiecesRBs();
            }

        }
    }
#endif


}