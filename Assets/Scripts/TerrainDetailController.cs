using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainDetailController : MonoBehaviour
{
    [SerializeField] [Range(1.0f, 1000.0f)] float PlaneWidth = 10.0f;
    [SerializeField] [Range(1.0f, 1000.0f)] float PlaneHeight = 10.0f;
    [SerializeField] [Range(1, 1000)] int xResolution = 10;
    [SerializeField] [Range(1, 1000)] int yResolution = 10;

    private void OnValidate()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        int vertCount = (xResolution + 1) * (yResolution + 1);
        //Pre allocate arrays for better performance
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] triangles = new int[xResolution * yResolution * 6];

        float xStep = PlaneWidth / xResolution;
        float yStep = PlaneHeight / yResolution;

        int vertIndex = 0;
        int triIndex = 0;

        for (int y = 0; y <= yResolution; y++)
        {
            for (int x = 0; x <= xResolution; x++)
            {
                vertices[vertIndex] = new Vector3(x * xStep, 0, y * yStep);
                uv[vertIndex] = new Vector2((float)x / xResolution, (float)y / yResolution);

                if (x < xResolution && y < yResolution)
                {
                    int topLeft = vertIndex;
                    int bottomLeft = vertIndex + xResolution + 1;
                    int bottomRight = bottomLeft + 1;
                    int topRight = topLeft + 1;

                    triangles[triIndex++] = topLeft;
                    triangles[triIndex++] = bottomLeft;
                    triangles[triIndex++] = topRight;

                    triangles[triIndex++] = topRight;
                    triangles[triIndex++] = bottomLeft;
                    triangles[triIndex++] = bottomRight;
                }

                vertIndex++;
            }
        }

        mesh.vertices = vertices; //Deep copy! 
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
