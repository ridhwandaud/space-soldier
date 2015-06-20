using UnityEngine;
using System.Collections;

public class BasicGun : MonoBehaviour {

    public float firingDelay = .1f;
    public float bulletSpeed = 50;

    private float nextFiringTime;
    private StackPool bulletPool;

	void Awake () {
        nextFiringTime = 0;
        bulletPool = GameObject.Find("BulletPool").GetComponent<StackPool>();
	}
	
	// Update is called once per frame
	void Update () {
        // 0 = left, 1 = right, 2 = middle
        if (Input.GetMouseButton(0) && Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + firingDelay;
            GameObject bullet = bulletPool.Pop();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            
            // Fire bullet towards mouse pointer.
            Vector3 mouse = Input.mousePosition;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 direction = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y).normalized;

            // this has to be done before setting velocity or it won't work.
            bullet.SetActive(true);

            bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        }
	}
}
