using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGPUInstancing : MonoBehaviour
{
    [SerializeField] Mesh TreeMesh;
    [SerializeField] List<Material> TreeMaterial;
    [SerializeField] TerrainDetailController terrainController;
    [SerializeField] ComputeShader TRSGenerator;
    [SerializeField] float NoiseSmoothness = 50.0f;
    [SerializeField] float HeightThreshold = 0.5f;

    private ComputeBuffer vertexBuffer;
    private ComputeBuffer trsMatricesBuffer;
    private ComputeBuffer counterBuffer;

    Vector3[] vertices;
    Matrix4x4[] trsMatrices;
    int kernelId;
    Camera mainCamera;

    private void OnDestroy()
    {
        vertexBuffer.Release();
        trsMatricesBuffer.Release();
        counterBuffer.Release();
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        vertices = terrainController?.GetVertices();
        trsMatrices = new Matrix4x4[vertices.Length];

        vertexBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);
        vertexBuffer.SetData(vertices);

        trsMatricesBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 16);

        counterBuffer = new ComputeBuffer(1, sizeof(int));
        counterBuffer.SetData(new int[] { 0 });
        kernelId = TRSGenerator.FindKernel("CSMain");


    }

    // Update is called once per frame
    void Update()
    {
        TRSGenerator.SetBuffer(kernelId, "objectVertices", vertexBuffer);
        TRSGenerator.SetBuffer(kernelId, "trsMatrices", trsMatricesBuffer);
        TRSGenerator.SetBuffer(kernelId, "counterBuffer", counterBuffer);
        TRSGenerator.SetMatrix("objToWorldMatrix", terrainController.transform.localToWorldMatrix);
        TRSGenerator.SetFloat("smoothness", NoiseSmoothness);
        TRSGenerator.SetFloat("threshold", HeightThreshold);

        int threadGroupsX = Mathf.CeilToInt(vertices.Length / 10.0f);
        TRSGenerator.Dispatch(kernelId, threadGroupsX, 1, 1);

        // Retrieve results (optional)
        trsMatricesBuffer.GetData(trsMatrices);

        int[] counterData = new int[1];
        counterBuffer.GetData(counterData);
        Debug.Log("Counter: " + counterData[0]);
        for(int i=0; i<TreeMaterial.Count; i++)
        {
            Graphics.DrawMeshInstanced(TreeMesh, i, TreeMaterial[i], trsMatrices, counterData[0]);
        }

        counterBuffer.SetData(new int[] { 0 });
        trsMatricesBuffer.Release();
        trsMatricesBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 16);
    }
}
