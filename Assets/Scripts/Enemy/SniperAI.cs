using UnityEngine;
using System.Collections;

public class SniperAI : EnemyAI {

    public int range;
    public float timeBetweenShots;

    private float nextFiringTime = 0;
    private bool isPreparingForShot = false;
    private Animator animator;

    // TODO: Delete. This is just for testing.
    private float projectileSpeed = 10f;
    private StackPool projectilePool;

	void Awake () {
        projectilePool = GameObject.Find("FireballPool").GetComponent<StackPool>();
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = Player.PlayerTransform.position;

        float distanceFromPlayer = Vector3.Distance(playerPosition, enemyPosition);

        if (EnemyUtil.CanSee(transform.position, playerPosition) && distanceFromPlayer <= range && 
            Time.time > nextFiringTime && !isPreparingForShot)
        {
            StartCharging();
        }

        if (isPreparingForShot)
        {
            if(EnemyUtil.CanSee(transform.position, playerPosition) && distanceFromPlayer <= range)
            {
                // turn sprite and gun to face player.
            }
            else
            {
                StopCharging();
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player Projectile")
        {

        }
    }

    void Fire()
    {
        StopCharging();
        nextFiringTime = Time.time + timeBetweenShots;

        GameObject projectile = projectilePool.Pop();
        projectile.transform.position = gameObject.transform.position;

        Vector3 offset = Player.PlayerTransform.position - gameObject.transform.position;
        float rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg + 90;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));

        projectile.SetActive(true);
        projectile.GetComponent<Rigidbody2D>().velocity = projectileSpeed * offset;
    }

    void StartCharging()
    {
        isPreparingForShot = true;
        animator.SetBool("Charging", true);
    }

    void StopCharging()
    {
        isPreparingForShot = false;
        animator.SetBool("Charging", false);
    }
}
