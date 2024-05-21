using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainGPUInstancing : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] Mesh instanceMesh;
    public static TerrainGPUInstancing Instance { get; private set; }
    List<Matrix4x4> instanceMatrices = new List<Matrix4x4>();
    Matrix4x4[] _instanceMatrices = new Matrix4x4[256];
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public int AddTerrainChunkInstance(Matrix4x4 trsMatrix)
    {
        instanceMatrices.Add(trsMatrix);
        return instanceMatrices.Count - 1; //This will serve as an ID for the instance
    }

    public void RemoveTerrainChunkInstance(int id)
    {
        instanceMatrices.RemoveAt(id);
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMeshInstanced(instanceMesh, 0, material, instanceMatrices);
    }
}
