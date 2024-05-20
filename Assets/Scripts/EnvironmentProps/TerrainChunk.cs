using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour, IPoolable
{
    [SerializeField] GameObject terrainObject;
    [SerializeField] private Vector2 size;
    public Vector2 Size { get { return size; } }
    public void OnRetrieveFromPool()
    {
    }

    public void OnReturnToPool()
    {
    }
    private void Awake()
    {
        if (terrainObject != null)
        {
            var meshFilter = terrainObject.GetComponent<MeshFilter>();
            if(meshFilter != null)
            {
                size.x = meshFilter.mesh.bounds.size.x;
                size.y = meshFilter.mesh.bounds.size.z;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
