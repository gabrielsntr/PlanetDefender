using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectPrefabs;

    private List<GameObject> pooledObjects = new List<GameObject>();

    public GameObject GetObject(string type)
    {
        foreach (GameObject go in pooledObjects)
        {
            if (go.name == type && !go.activeInHierarchy)
            {
                go.SetActive(true);
                return go;
            }
        }
        foreach (GameObject gameObject in objectPrefabs)
        {
            if (gameObject.name == type)
            {
                GameObject newObject = Instantiate(gameObject);
                pooledObjects.Add(newObject);
                newObject.name = type;
                return newObject;
            }
        }
        return null;
    }
    public void ReleaseObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
