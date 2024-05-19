using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProceduralTerrainShaderController : MonoBehaviour
{
    [SerializeField] [Range(0.001f, 100.0f)] float heightMultiplier = 10.0f;
    public struct VertexData
    {
        public Vector3 position;
        public VertexData(Vector3 position)
        {
            this.position = position;
        }
    }

    Vector2 cameraPosition = new Vector2(0, 0);
    int XResolution = 0;
    int YResolution = 0;
    Material perlinTerrainMaterial = null;
    void Start()
    {
        InitializeShaderParameters();
    }

    private void InitializeShaderParameters()
    {
        //TerrainDetailController terrainDetailController = GetComponent<TerrainDetailController>();
        //XResolution = terrainDetailController.XResolution;
        //YResolution = terrainDetailController.YResolution;
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
        /*
         Shader properties:
            _CameraPosition ("Camera Position", Vector) = (0,0,0,0)
            _HeightMultiplier ("Height Multiplier", Float) = 1.0
            _MainTex ("Albedo (RGB)", 2D) = "white" {}
            _Color ("Color", Color) = (1,1,1,1)
         */
        perlinTerrainMaterial.SetFloat("_HeightMultiplier", heightMultiplier);

    }

    void InitVertexArray()
    {

    }
}
