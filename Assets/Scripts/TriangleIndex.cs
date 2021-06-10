// This script draws a debug line around mesh triangles
// as you move the mouse over them.
using UnityEngine;
using System.Collections;

public class TriangleIndex : MonoBehaviour
{
    Camera cam;
    public MeshCollider mc;
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (p1 != Vector3.zero && p2 != Vector3.zero && p3 != Vector3.zero)
        {
            Debug.DrawLine(p1, p2, Color.red);
            Debug.DrawLine(p2, p3, Color.red);
            Debug.DrawLine(p3, p1, Color.red);
        }
        
        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;
        
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;

        Debug.Log($"Index: {hit.triangleIndex}");
        if (hit.triangleIndex == -1) return;

        Mesh mesh = mc.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        p1 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        p2 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        p3 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        p3 = hitTransform.TransformPoint(p3);
    }
}