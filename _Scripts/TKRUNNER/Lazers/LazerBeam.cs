using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
namespace TKRunner
{
    public class LazerBeam : MonoBehaviour, ISclicing
    {
        [SerializeField] private LineRenderer linePF;
       private LineRenderer line;
        [SerializeField] private bool DirectPoints;
        [SerializeField] private float Length;
        [SerializeField] private Collider coll;
        [SerializeField] private Transform p1;
        [SerializeField] private Transform p2;
        public void Init()
        {
            coll.isTrigger = true;

            if (linePF == null) return;

            line = Instantiate(linePF);
            line.transform.position = Vector3.zero;
            line.transform.eulerAngles = Vector3.zero;
            if (p1 == null && p2 == null)
            {
                p1 = transform.parent.GetChild(1).transform;
                p2 = transform.parent.GetChild(2).transform;
            }

            UpdateLine();
        }
        private void UpdateLine()
        {
            if (line == null) return;
            Vector3[] pos = new Vector3[2];
            if (DirectPoints)
            {
                pos[0] = p1.position;
                pos[1] = p2.position;
            }
            else
            {
                pos[0] = p1.position;
                pos[1] = p1.position + (p2.position - p1.position).normalized * Length;
            }
   
            line.SetPositions(pos);
        }
        private void Update()
        {
            if (linePF != null)
                UpdateLine();
        }

        public void Activate()
        {
            coll.enabled = true;

        }
        public void DeActivate()
        {
            coll.enabled = false;
        }

        public Plane GetSlicePlane()
        {
            return new Plane(Vector3.up, transform.position);
        }

        private void OnDisable()
        {
            if(line!= null)
                Destroy(line.gameObject);
        }
        private void OnDestroy()
        {
            if (line != null)
                Destroy(line.gameObject);
        }
    }
}