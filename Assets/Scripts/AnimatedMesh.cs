using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class AnimatedMesh : MonoBehaviour
{

    public Vector3[] verts;

    private MeshFilter meshComponent;
    private Mesh mesh;

    // Start is called before the first frame update
    [ExecuteInEditMode]
    void Start()
    {
        TryGetComponent(out meshComponent);
        mesh = new Mesh();
    }

    // Update is called once per frame
    [ExecuteInEditMode]
    void Update()
    {
        meshComponent.mesh = mesh;
        mesh.vertices = verts;
        mesh.triangles = GetTriangles();
    }

    private int[] cachedTriangles;

    private int[] GetTriangles()
    {
        if (cachedTriangles == null || cachedTriangles.Length != (verts.Length / 3) * 3)
        {
            cachedTriangles = new int[verts.Length];
            for (int i = 0; i < (verts.Length / 3) * 3; i++)
            {
                cachedTriangles[i] = i;
            }
        }
        return cachedTriangles;
    }
}
