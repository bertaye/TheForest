using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainDetailController : MonoBehaviour
{
    [SerializeField]
    [Range(1.0f, 1000.0f)]
    private float planeWidth = 10.0f;

    [SerializeField]
    [Range(1.0f, 1000.0f)]
    private float planeHeight = 10.0f;

    [SerializeField]
    [Range(1, 1000)]
    private int xResolution = 10;

    [SerializeField]
    [Range(1, 1000)]
    private int yResolution = 10;

    public float PlaneWidth
    {
        get => planeWidth;
        private set => planeWidth = value;
    }

    public float PlaneHeight
    {
        get => planeHeight;
        private set => planeHeight = value;
    }

    public int XResolution
    {
        get => xResolution;
        private set => xResolution = value;
    }

    public int YResolution
    {
        get => yResolution;
        private set => yResolution = value;
    }

    private void OnValidate()
    {
        GenerateMesh();
    }
    private void GenerateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        int vertCount = (xResolution + 1) * (yResolution + 1);
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] triangles = new int[xResolution * yResolution * 6];

        float xStep = PlaneWidth / xResolution;
        float yStep = PlaneHeight / yResolution;

        int vertIndex = 0;
        int triIndex = 0;

        // Generate vertices and UVs
        for (int y = 0; y <= yResolution; y++)
        {
            for (int x = 0; x <= xResolution; x++)
            {

            }
        }

        // Generate triangles
        for (int y = 0; y < yResolution; y++)
        {
            for (int x = 0; x < xResolution; x++)
            {
                /* Here is how we create vertices:
                 *  v1             -- v2            -- v3              ... -- vXResolution
                 *  vXResolution+1 -- vXResolution+2 -- vXResolution+3 ... -- v(2*XResolution)
                 *  .                                                      .
                 *  .                                                      .
                 *  .                                                      .
                 *  ....                                               ... -- V(XResolution*YResolution)
                 *  
                 *  So, we will create triangles in CCW, we can create 2 triangles for one vertex for simplicity,
                 *  But this means we should not create triangles for left most and bottom most vertices
                 *  
                 */

                vertices[vertIndex] = new Vector3(x * xStep, 0, y * yStep);
                uv[vertIndex] = new Vector2((float)x / xResolution, (float)y / yResolution);
                vertIndex++;


                if(x == (xResolution - 1) || y == (yResolution - 1))
                {
                    //We will not create triangles for the last row and column because that would break the plane mesh
                    continue;
                }

                int current = y * (xResolution) + x;
                int right = current + 1;
                int bottom = current + xResolution; //This is the vertex below the current vertex
                int bottomRight = bottom + 1;

                triangles[triIndex++] = current;
                triangles[triIndex++] = bottom;
                triangles[triIndex++] = right;

                triangles[triIndex++] = right;
                triangles[triIndex++] = bottom;
                triangles[triIndex++] = bottomRight;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
