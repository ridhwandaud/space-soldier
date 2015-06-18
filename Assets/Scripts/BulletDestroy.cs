using UnityEngine;
using System.Collections;

public class BulletDestroy : MonoBehaviour {

	void OnEnable () {
        Invoke("Destroy", 2f);
	}

    public void Destroy()
    {
        CancelInvoke("Destroy");
        gameObject.SetActive(false);
        StackPool.current.Push(gameObject);
    }
}
