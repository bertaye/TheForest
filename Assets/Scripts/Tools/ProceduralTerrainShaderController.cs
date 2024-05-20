using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrainShaderController : MonoBehaviour
{
    [SerializeField] [Range(0.001f, 100.0f)] float heightMultiplier = 10.0f;

    Material perlinTerrainMaterial = null;
    void Start()
    {
        InitializeShaderParameters();
    }

    private void InitializeShaderParameters()
    {
        perlinTerrainMaterial = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        GenerateTerrain();
    }

    void OnDestroy()
    {
    }

    void GenerateTerrain()
    {
        perlinTerrainMaterial.SetFloat("_HeightMultiplier", heightMultiplier);
        perlinTerrainMaterial.EnableKeyword("_MAIN_LIGHT_SHADOWS");

    }

    void InitVertexArray()
    {

    }
}
