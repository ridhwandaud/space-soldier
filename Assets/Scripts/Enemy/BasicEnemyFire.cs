using UnityEngine;
using System.Collections;

public class BasicEnemyFire : MonoBehaviour {

    public float fireInterval = 2f;
    public float projectileSpeed = 10f;
    public string projectilePoolName;

    private StackPool projectilePool;
    private GameObject player;
    private float nextFireTime;

	void Start () {
        player = GameObject.Find("Soldier");
        nextFireTime = Time.time + .5f;
        projectilePool = GameObject.Find(projectilePoolName).GetComponent<StackPool>();
	}
	
	public void Fire () {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireInterval;
            GameObject projectile = projectilePool.Pop();
            projectile.transform.position = gameObject.transform.position;

            Vector3 offset = player.transform.position - gameObject.transform.position;
            float rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg + 90;
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));

            projectile.SetActive(true);

            projectile.GetComponent<Rigidbody2D>().velocity = projectileSpeed * offset.normalized;
        }
	}
}
