using UnityEngine;

public enum NoiseType
{
    SimplexNoise = 0,
    PerlinNoise = 1
}
public class ProceduralTerrainComputeDispatcher : MonoBehaviour
{
    [SerializeField] ComputeShader heightComputeShader;
    [SerializeField] MeshFilter meshFilter;
    public float heightMultiplier = 1.0f;
    public float noiseSmoothness = 10.0f;
    [SerializeField] NoiseType noiseType = 0; // 0 for SimplexNoise, 1 for PerlinNoise

    void Start()
    {
        ModifyHeight();
    }

    private void OnValidate()
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
        heightComputeShader.SetBuffer(0, "vertices", vertexBuffer);
        heightComputeShader.SetFloat("_HeightMultiplier", heightMultiplier);
        heightComputeShader.SetFloat("_NoiseSmoothness", noiseSmoothness);
        heightComputeShader.SetInt("_NoiseType", (int)noiseType);

        heightComputeShader.SetMatrix("_ModelMatrix", meshFilter.transform.localToWorldMatrix);

        int threadGroups = Mathf.CeilToInt(vertexCount / 256.0f);
        heightComputeShader.Dispatch(0, threadGroups, 1, 1);

        vertexBuffer.GetData(vertexArray);
        vertexBuffer.Release();

        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] = vertexArray[i].position;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
    }
}
