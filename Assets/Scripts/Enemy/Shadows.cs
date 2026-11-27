using System;
using UnityEngine;

public class Shadows : MonoBehaviour
{
    RaycastHit hit;
    LayerMask layerMask;
    [SerializeField]GameObject shadow;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Floor");
    }

    void LateUpdate()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) 
        {
            shadow.transform.position = hit.point;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
    }
}
