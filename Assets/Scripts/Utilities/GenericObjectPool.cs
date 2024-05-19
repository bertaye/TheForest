using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
public class GenericObjectPool<T> where T : class
{
    public delegate T FactoryMethod(T obj);
    public delegate void PoolCallback(T obj);

    public FactoryMethod OnCreate;
    public PoolCallback OnRetrieve;
    public PoolCallback OnReturn;
    
        [SerializeField] private T obj;
    [SerializeField] private int poolSize;
    private Queue<T> pool;

    public GenericObjectPool(T obj, FactoryMethod onCreate, PoolCallback onRetrieve, PoolCallback onReturn, int poolSize = 64)
    {
        this.obj = obj;
        this.poolSize = poolSize;
        this.OnCreate = onCreate;
        this.OnRetrieve = onRetrieve;
        this.OnReturn = onReturn;
        InitializePool();
    }
    private void InitializePool()
    {
        pool = new Queue<T>();
        for (int i = 0; i < poolSize; i++)
        {
            if(OnCreate != null)
            {
                pool.Enqueue(OnCreate(this.obj));
            }
        }
    }
    public T RetrieveFromPool()
    {
        if(pool.Count == 0)
        {   for(int i = 0; i < poolSize/2; i++)
            {
                pool.Enqueue(OnCreate(this.obj));
            }
        }
        T obj = pool.Dequeue();
        if(OnRetrieve != null)
        {
            OnRetrieve(obj);
        }
        return obj;
    }

    public void ReturnToPool(T obj)
    {
        if(OnReturn != null)
        {
            OnReturn(obj);
        }
        pool.Enqueue(obj);
    }
}
}
