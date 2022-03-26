using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Commongame.Beizer
{

    public class BezierCurve : MonoBehaviour
    {
		public List<Transform> points = new List<Transform>();


		public Vector3 GetPosLocal(int ind)
        {
			if (points.Count <= ind)
				return Vector3.zero;
			return points[ind].localPosition;
        }


		public void SetPosLocal(Vector3 point, int ind)
        {
			if (points.Count <= ind)
				return;
			points[ind].localPosition = point;
		}





        public Vector3 GetPoint(float t)
        {
			return transform.TransformPoint(Bezier.GetPointQuadratic(points[0].localPosition, 
				points[1].localPosition, 
				points[2].localPosition, t));
		}

		public Vector3 GetPoint(List<Vector3> positions, float t)
        {
			return transform.TransformPoint(Bezier.GetPointQuadratic(positions[0],
	positions[1],
	positions[2], t));
		}

    }






#if UNITY_EDITOR

	[CustomEditor(typeof(BezierCurve))]
	public class BezierCurveInspector : Editor
	{

		private BezierCurve curve;
		private Transform handleTransform;
		private Quaternion handleRotation;
		private const int lineSteps = 10;

		private void OnSceneGUI()
		{
			curve = target as BezierCurve;
			handleTransform = curve.transform;
			handleRotation = Tools.pivotRotation == PivotRotation.Local ?
				handleTransform.rotation : Quaternion.identity;

			Vector3 p0 = curve.points[0].position;
			Vector3 p1 = curve.points[1].position;
			Vector3 p2 = curve.points[2].position;

			Handles.color = Color.white;
			Vector3 lineStart = curve.GetPoint(0f);
			for (int i = 1; i <= lineSteps; i++)
			{
				Vector3 lineEnd = curve.GetPoint(i / (float)lineSteps);
				Handles.DrawLine(lineStart, lineEnd);
				lineStart = lineEnd;
			}
		}
	}

#endif

}