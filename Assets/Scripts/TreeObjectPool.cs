using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Singletons
{
public class TreeObjectPool : MonoBehaviour, IObjectPooler
{
    public GameObject GetObjectFromPool()
    {
        throw new System.NotImplementedException();
    }

    public void ReturnObjectToPool(GameObject objectToReturn)
    {
        throw new System.NotImplementedException();
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
}
