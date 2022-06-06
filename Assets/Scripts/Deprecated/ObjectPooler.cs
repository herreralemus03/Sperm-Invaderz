/*using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { private set; get; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PoolObject(GameObject gameObjectToPool, Vector3 position, List<GameObject> gameObjects, Transform parent = null)
    {
        var itemToInstance = FindItemsAvailable(gameObjects);
        if (itemToInstance != null && gameObjects.Count >= 3)
        {
            //itemToInstance.transform.position = position;
            itemToInstance.SetActive(true);
            return;
        }
        gameObjects.Add(gameObjectToPool);
        Instantiate(gameObjectToPool, position, Quaternion.identity, parent);
    }

    private GameObject FindItemsAvailable(List<GameObject> gameObjects)
    {
        //return gameObjects.Find((obj) => !obj.activeInHierarchy);
        return gameObjects.FirstOrDefault(ob => !ob.activeInHierarchy);
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}*/