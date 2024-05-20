#if UNITY_2021_1_OR_NEWER
#define BUILTIN_OBJECT_POOL
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if BUILTIN_OBJECT_POOL
using UnityEngine.Pool;
#endif
using Utilities;


public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] GameObject TerrainChunkPrefab;
    [SerializeField] GameObject TreePrefab;
    [SerializeField] GameObject RockPrefab;
    [SerializeField] GameObject GrassPrefab;
    [SerializeField] int terrainChunkPoolSize = 10;
    [SerializeField] int treePoolSize;
    [SerializeField] int rockPoolSize;
    [SerializeField] int grassPoolSize;

    public int TerrainChunkPoolSize { get => terrainChunkPoolSize; }
    public int TreePoolSize { get => treePoolSize; }
    public int RockPoolSize { get => rockPoolSize; }
    public int GrassPoolSize { get => grassPoolSize; }

#if BUILTIN_OBJECT_POOL
    ObjectPool<GameObject> TerrainChunkPool;
    ObjectPool<GameObject> TreePool;
    ObjectPool<GameObject> RockPool;
    ObjectPool<GameObject> GrassPool;
#else
    GenericObjectPool<GameObject> TerrainChunkPool_Legacy;
    GenericObjectPool<GameObject> TreePool_Legacy;
    GenericObjectPool<GameObject> RockPool_Legacy;
    GenericObjectPool<GameObject> GrassPool_Legacy;
#endif
    public static ObjectPoolManager Instance { get; private set; }

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

        InitializeObjectPools();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetTerrainChunk()
    {
        #if BUILTIN_OBJECT_POOL
        return TerrainChunkPool.Get();
        #else
        return TerrainChunkPool_Legacy.RetrieveFromPool();
        #endif
    }

    public void ReturnTerrainChunkToPool(GameObject terrainChunk)
    {
        #if BUILTIN_OBJECT_POOL
        TerrainChunkPool.Release(terrainChunk);
#else
        TerrainChunkPool_Legacy.ReturnToPool(terrainChunk);
#endif
        return;
    }

    public GameObject GetTree()
    {
        #if BUILTIN_OBJECT_POOL
        return TreePool.Get();
        #else
        return TreePool_Legacy.RetrieveFromPool();
        #endif
    }

    public void ReturnTreeToPool(GameObject tree)
    {
        #if BUILTIN_OBJECT_POOL
        TreePool.Release(tree);
        #else
        TreePool_Legacy.ReturnToPool(tree);
        #endif
        return;
    }

    public GameObject GetRock()
    {
        #if BUILTIN_OBJECT_POOL
        return RockPool.Get();
        #else
        return RockPool_Legacy.RetrieveFromPool();
        #endif
    }

    public void ReturnRockToPool(GameObject rock)
    {
        #if BUILTIN_OBJECT_POOL
        RockPool.Release(rock);
        #else
        RockPool_Legacy.ReturnToPool(rock);
        #endif
        return;
    }

    public GameObject GetGrass()
    {
        #if BUILTIN_OBJECT_POOL
        return GrassPool.Get();
        #else
        return GrassPool_Legacy.RetrieveFromPool();
        #endif
    }

    public void ReturnGrassToPool(GameObject grass)
    {
        #if BUILTIN_OBJECT_POOL
        GrassPool.Release(grass);
        #else
        GrassPool_Legacy.ReturnToPool(grass);
        #endif
        return;
    }

    void InitializeObjectPools()
    {
#if BUILTIN_OBJECT_POOL
        TerrainChunkPool = new ObjectPool<GameObject>(
            createFunc: () => { return Instantiate(TerrainChunkPrefab); },
            actionOnGet: (obj) => { obj.SetActive(true); 
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnRetrieveFromPool();
                }
            },
            actionOnRelease: (obj) => { obj.SetActive(false); 
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnReturnToPool();
                }
            },
        actionOnDestroy: (obj) => { Destroy(obj); },
        collectionCheck: true,
        defaultCapacity: terrainChunkPoolSize,
        maxSize: 100);

        TreePool = new ObjectPool<GameObject>(
            createFunc : () => { return Instantiate(TreePrefab); },
            actionOnGet : (obj) => { obj.SetActive(true); 
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnRetrieveFromPool();
                }
            },
            actionOnRelease : (obj) => { obj.SetActive(false); 
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnReturnToPool();
                }
            },
            actionOnDestroy : (obj) => { Destroy(obj); },
            collectionCheck : true,
            defaultCapacity : treePoolSize,
            maxSize : 10000
            );

        RockPool = new ObjectPool<GameObject>(
            createFunc: () => { return Instantiate(RockPrefab); },
            actionOnGet: (obj) => { obj.SetActive(true); 
                                    var poolable = obj.GetComponent<IPoolable>();
                                    if (poolable != null)
                                    {
                                        poolable.OnRetrieveFromPool();
                                    }
                                    },

            actionOnRelease: (obj) => { obj.SetActive(false);
                                    var poolable = obj.GetComponent<IPoolable>();
                                    if (poolable != null)
                                    {
                                        poolable.OnReturnToPool();
                                    }
                                    },
            actionOnDestroy: (obj) => { Destroy(obj); },
            collectionCheck: true,
            defaultCapacity: rockPoolSize,
            maxSize: 10000
            );

        GrassPool = new ObjectPool<GameObject>(
            createFunc: () => { return Instantiate(GrassPrefab); },
            actionOnGet: (obj) => { obj.SetActive(true); 
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnRetrieveFromPool();
                }
            },
            actionOnRelease: (obj) => { obj.SetActive(false);
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnReturnToPool();
                }
            },
            actionOnDestroy: (obj) => { Destroy(obj); },
            collectionCheck: true,
            defaultCapacity: rockPoolSize,
            maxSize: 10000
            );
#else
        TerrainChunkPool_Legacy = new GenericObjectPool<GameObject>(
            obj: TerrainChunkPrefab,
            onCreate: (obj) => { return Instantiate(obj); },
            onRetrieve: (obj) => { obj.SetActive(true); 
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnRetrieveFromPool();
                }
            },
            onReturn: (obj) => { obj.SetActive(false);
                    var poolable = obj.GetComponent<IPoolable>();
                    if (poolable != null)
                {
                    poolable.OnReturnToPool();
                }
            },
            poolSize: terrainChunkPoolSize);

        TreePool_Legacy = new GenericObjectPool<GameObject>(
            obj: TreePrefab,
            onCreate: (obj) => { return Instantiate(obj); },
            onRetrieve: (obj) => { obj.SetActive(true); 
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnRetrieveFromPool();
                }
            },
            onReturn: (obj) => { obj.SetActive(false);
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnReturnToPool();
                }
            },
            poolSize: treePoolSize
            );

        RockPool_Legacy = new GenericObjectPool<GameObject>(
            obj: RockPrefab,
            onCreate: (obj) => { return Instantiate(obj); },
            onRetrieve: (obj) => {
                obj.SetActive(true);
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnRetrieveFromPool();
                }
            },
            onReturn: (obj) => {
                obj.SetActive(false);
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnReturnToPool();
                }
            }, 
            poolSize: rockPoolSize
            );

        GrassPool_Legacy = new GenericObjectPool<GameObject>(
            obj: GrassPrefab,
            onCreate: (obj) => { return Instantiate(obj); },
            onRetrieve: (obj) => {
                obj.SetActive(true);
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnRetrieveFromPool();
                }
            },
            onReturn: (obj) => {
                obj.SetActive(false);
                var poolable = obj.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnReturnToPool();
                }
            }, 
            poolSize: grassPoolSize
            );
#endif
    }
}
