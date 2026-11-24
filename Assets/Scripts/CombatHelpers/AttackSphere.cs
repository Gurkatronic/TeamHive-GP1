using System.Linq;
using UnityEngine;

public class AttackSphere : MonoBehaviour
{
    float range = 2f;
    float radius = 3f;
    public Vector3 sphereOffset;
    public LayerMask attackLayers;

    public Collider[] Attack() {
        Debug.Log("Calling attack");
        return Physics.OverlapSphere(transform.position + sphereOffset, radius, attackLayers);
    }

    private void Update() {
        DrawDebugSphere(transform.position, radius, Color.red);
    }

    private void DrawDebugSphere(Vector3 center, float sphereRadius, Color color, int segments = 24) {
        float step = 360f / segments;
        for (int i = 0; i < segments; i++) {
            float angleA = Mathf.Deg2Rad * (i * step);
            float angleB = Mathf.Deg2Rad * ((i + 1) * step);

            Vector3 flatA = new Vector3(Mathf.Cos(angleA), 0f, Mathf.Sin(angleA)) * sphereRadius;
            Vector3 flatB = new Vector3(Mathf.Cos(angleB), 0f, Mathf.Sin(angleB)) * sphereRadius;

            Vector3 vertXA = new Vector3(0f, Mathf.Cos(angleA), Mathf.Sin(angleA)) * sphereRadius;
            Vector3 vertXB = new Vector3(0f, Mathf.Cos(angleB), Mathf.Sin(angleB)) * sphereRadius;

            Vector3 vertZA = new Vector3(Mathf.Cos(angleA), Mathf.Sin(angleA), 0f) * sphereRadius;
            Vector3 vertZB = new Vector3(Mathf.Cos(angleB), Mathf.Sin(angleB), 0f) * sphereRadius;

            Debug.DrawLine(center + flatA, center + flatB, color);
            Debug.DrawLine(center + vertXA, center + vertXB, color);
            Debug.DrawLine(center + vertZA, center + vertZB, color);
        }
    }
}
