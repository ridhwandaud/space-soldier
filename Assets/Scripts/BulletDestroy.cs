using UnityEngine;
using System.Collections;

public class BulletDestroy : MonoBehaviour {

	void OnEnable () {
        Invoke("Destroy", 2f);
	}

    void Destroy()
    {
        gameObject.SetActive(false);
        StackPool.current.Push(gameObject);
    }
}
