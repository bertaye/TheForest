using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class TerrainDrawController : MonoBehaviour
{
    [SerializeField] Vector2 TerrainChunkSize = new Vector2(100.0f, 100.0f);
    CameraController cameraController;

    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i=-5; i < 5; i++)
        {
            TerrainGPUInstancing.Instance.AddTerrainChunkInstance(Matrix4x4.TRS(
                gameObject.transform.position + new Vector3(i * TerrainChunkSize.x, 0, 0), 
                gameObject.transform.rotation, gameObject.transform.localScale));
        }
        
    }

    //We need to ensure camera movement is completed
    void LateUpdate()
    {
        
    }
}
