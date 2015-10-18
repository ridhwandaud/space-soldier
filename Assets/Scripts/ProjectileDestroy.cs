using UnityEngine;

public class ProjectileDestroy : MonoBehaviour {

    public string poolName;

    private StackPool stackPool;

    void Awake()
    {
        stackPool = GameObject.Find(poolName).GetComponent<StackPool>();
    }

    public void Destroy()
    {
        CancelInvoke("Destroy");
        gameObject.SetActive(false);
        stackPool.Push(gameObject);
    }
}
