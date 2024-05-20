using UnityEngine;
using System.IO;

public class NoiseTextureToMeshDispatcher : MonoBehaviour
{
    public ComputeShader heightComputeShader;
    public MeshFilter meshFilter;
    public Texture2D noiseTexture;
    public float heightMultiplier = 1.0f;

    void Start()
    {
        ModifyHeight();
    }

    void ModifyHeight()
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int vertexCount = vertices.Length;

        ComputeBuffer vertexBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 8);
        Vertex[] vertexArray = new Vertex[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            vertexArray[i] = new Vertex
            {
                position = vertices[i],
                normal = mesh.normals[i],
                uv = mesh.uv[i]
            };
        }

        vertexBuffer.SetData(vertexArray);
        int kernelIndex = heightComputeShader.FindKernel("CSMain2");
        heightComputeShader.SetBuffer(kernelIndex, "vertices", vertexBuffer);
        heightComputeShader.SetFloat("_HeightMultiplier", heightMultiplier);
        heightComputeShader.SetMatrix("_ModelMatrix", meshFilter.transform.localToWorldMatrix);

        int noiseTexID = Shader.PropertyToID("_NoiseTex");
        heightComputeShader.SetTexture(kernelIndex, noiseTexID, noiseTexture);

        int threadGroups = Mathf.CeilToInt(vertexCount / 256.0f);
        heightComputeShader.Dispatch(kernelIndex, threadGroups, 1, 1);

        vertexBuffer.GetData(vertexArray);
        vertexBuffer.Release();

        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] = vertexArray[i].position;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        SaveMesh(mesh, "Assets/3D Objects/GeneratedMesh.obj");
    }

    void SaveMesh(Mesh mesh, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(MeshToString(mesh));
        }
    }

    string MeshToString(Mesh mesh)
    {
        StringWriter stringWriter = new StringWriter();

        stringWriter.WriteLine("g " + mesh.name);

        foreach (Vector3 v in mesh.vertices)
        {
            stringWriter.WriteLine("v " + v.x + " " + v.y + " " + v.z);
        }
        stringWriter.WriteLine();

        foreach (Vector3 v in mesh.normals)
        {
            stringWriter.WriteLine("vn " + v.x + " " + v.y + " " + v.z);
        }
        stringWriter.WriteLine();

        foreach (Vector2 v in mesh.uv)
        {
            stringWriter.WriteLine("vt " + v.x + " " + v.y);
        }
        stringWriter.WriteLine();

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                stringWriter.WriteLine(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}",
                    triangles[j] + 1, triangles[j + 1] + 1, triangles[j + 2] + 1));
            }
        }

        return stringWriter.ToString();
    }

    struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
    }
}
