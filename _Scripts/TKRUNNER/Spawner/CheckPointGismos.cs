using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CheckPointGismos : MonoBehaviour
{
    public float spawnRadius = 1f;
    private void OnDrawGizmosSelected()
    {
        Debug.Log("gismos");
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }
}
