using System;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;

public class Shadows : MonoBehaviour
{
    RaycastHit hit;
    LayerMask layerMask;
    [SerializeField] GameObject shadow;
    [SerializeField] GameObject enemyBody;

    Quaternion rot;
    private void Awake()
    {
        rot = shadow.transform.rotation;
        layerMask = LayerMask.GetMask("Floor");
    }

    void LateUpdate()
    {
        if (Physics.Raycast(enemyBody.transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            shadow.transform.position = hit.point;
            shadow.transform.position += new Vector3(0, 0.05f, 0);
            shadow.transform.rotation = rot;
        }
    }
}
