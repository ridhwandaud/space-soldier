using UnityEngine;
using System.Collections;

public class ProjectileDestroy : MonoBehaviour {

    public string poolName;

    private StackPool stackPool;

    void Awake()
    {
        stackPool = GameObject.Find(poolName).GetComponent<StackPool>();
    }

	void OnEnable () {
        //Invoke("Destroy", 2f);
	}

    public void Destroy()
    {
        CancelInvoke("Destroy");
        gameObject.SetActive(false);
        stackPool.Push(gameObject);
    }
}
