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
    [SerializeField] GameObject TreePrefab;
    [SerializeField] GameObject RockPrefab;
    [SerializeField] GameObject GrassPrefab;
    [SerializeField] int treePoolSize;
    [SerializeField] int rockPoolSize;
    [SerializeField] int grassPoolSize;

    public int TreePoolSize { get => treePoolSize; }
    public int RockPoolSize { get => rockPoolSize; }
    public int GrassPoolSize { get => grassPoolSize; }


#if BUILTIN_OBJECT_POOL
    ObjectPool<GameObject> TreePool;
    ObjectPool<GameObject> RockPool;
    ObjectPool<GameObject> GrassPool;
//TODO: #else
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

    public GameObject GetTree()
    {
        #if BUILTIN_OBJECT_POOL
        return TreePool.Get();
        //TODO: #else
        return TreePool_Legacy.RetrieveFromPool();
        #endif
    }

    public void ReturnTreeToPool(GameObject tree)
    {
        #if BUILTIN_OBJECT_POOL
        TreePool.Release(tree);
        //TODO: #else
        TreePool_Legacy.ReturnToPool(tree);
        #endif
        return;
    }

    public GameObject GetRock()
    {
        #if BUILTIN_OBJECT_POOL
        return RockPool.Get();
        //TODO: #else
        return RockPool_Legacy.RetrieveFromPool();
        #endif
    }

    public void ReturnRockToPool(GameObject rock)
    {
        #if BUILTIN_OBJECT_POOL
        RockPool.Release(rock);
        //TODO: #else
        RockPool_Legacy.ReturnToPool(rock);
        #endif
        return;
    }

    public GameObject GetGrass()
    {
        #if BUILTIN_OBJECT_POOL
        return GrassPool.Get();
        //TODO: #else
        return GrassPool_Legacy.RetrieveFromPool();
        #endif
    }

    public void ReturnGrassToPool(GameObject grass)
    {
        #if BUILTIN_OBJECT_POOL
        GrassPool.Release(grass);
        //TODO: #else
        GrassPool_Legacy.ReturnToPool(grass);
        #endif
        return;
    }

    void InitializeObjectPools()
    {
#if BUILTIN_OBJECT_POOL
        TreePool = new ObjectPool<GameObject>(
            createFunc : () => { return Instantiate(TreePrefab); },
            actionOnGet : (obj) => { obj.SetActive(true); },
            actionOnRelease : (obj) => { obj.SetActive(false); },
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
            actionOnGet: (obj) => { obj.SetActive(true); },
            actionOnRelease: (obj) => { obj.SetActive(false); },
            actionOnDestroy: (obj) => { Destroy(obj); },
            collectionCheck: true,
            defaultCapacity: rockPoolSize,
            maxSize: 10000
            );
//TODO: #else
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
