using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPooler 
{
    GameObject GetObjectFromPool();
    void ReturnObjectToPool(GameObject objectToReturn);
}
