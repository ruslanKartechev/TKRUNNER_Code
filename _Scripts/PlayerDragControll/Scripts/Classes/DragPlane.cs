using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPlane : MonoBehaviour
{
    [SerializeField] private Transform _upperPlane;
    [SerializeField] private Transform _lowerPlane;

    public void Init(float upperPlaneSize, float lowerPlaneSize)
    {
      //  _upperPlane.localScale = new Vector3(_upperPlane.localScale.x, upperPlaneSize * 0.5f, _upperPlane.localScale.z);
       // _lowerPlane.localScale = new Vector3(_lowerPlane.localScale.x, lowerPlaneSize * 0.5f, _lowerPlane.localScale.z);
    }
}
