using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackPool : MonoBehaviour {

    public static StackPool current;
    public GameObject pooledObject;
    public int pooledAmount = 20;
    public bool allowGrowth = true;

    private Stack<GameObject> pooledObjects;

    void Awake()
    {
        current = this;
    }

	void Start () {
        pooledObjects = new Stack<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject) as GameObject;
            obj.SetActive(false);
            pooledObjects.Push(obj);
        }
	}

    public GameObject Pop()
    {
        if (pooledObjects.Count > 0)
            return pooledObjects.Pop();

        if (allowGrowth)
        {
            print("Growing to " + pooledObjects.Count + 1);
            GameObject obj = Instantiate(pooledObject) as GameObject;
            return obj;
        }

        return null;
    }

    public void Push(GameObject obj)
    {
        pooledObjects.Push(obj);
    }
}
