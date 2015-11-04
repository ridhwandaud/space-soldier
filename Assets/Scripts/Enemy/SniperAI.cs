using UnityEngine;

public class SniperAI : EnemyAI {

    public float timeBetweenShots;
    public int damage;

    private float nextFiringTime = 0;
    private bool isPreparingForShot = false;
    private Animator animator;
    private Camera mainCam;
    private Wander wanderScript;
    private Rigidbody2D rb2d;

    // TODO: Refactor.
    private float projectileSpeed = 10f;
    private StackPool projectilePool;
    private bool sniperWasHit = false;

	void Awake () {
        projectilePool = GameObject.Find("FireballPool").GetComponent<StackPool>();
        animator = GetComponent<Animator>();
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        wanderScript = GetComponent<Wander>();
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	void Update () {
        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = Player.PlayerTransform.position;

        if(sniperWasHit)
        {
            sniperWasHit = false;
            StopCharging();
            return;
        }

        if (EnemyUtil.CanSee(transform.position, playerPosition))
        {
            if (IsVisible() && Time.time > nextFiringTime && !isPreparingForShot)
            {
                chasing = true;
                StartCharging();
            }
        } else
        {
            // TODO: Add some time before the next wander so that the sniper doesn't always move the second you get out of its view.
            chasing = false;
        }

        if (isPreparingForShot)
        {
            if (EnemyUtil.CanSee(transform.position, playerPosition) && IsVisible())
            {
                // turn sprite and gun to face player.
            }
            else
            {
                StopCharging();
            }
        } else
        {
            wanderScript.DoWander();
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player Projectile")
        {
            // Trigger detection happens before the Update step, which happens before animation updates. So we must make sure not to set
            // flags here that can be steamrolled over in the update loop.
            sniperWasHit = true;
        }
    }

    // TODO: Remove code duplication
    void Fire()
    {
        StopCharging();
        nextFiringTime = Time.time + timeBetweenShots;

        GameObject projectile = projectilePool.Pop();
        projectile.GetComponent<BasicEnemyProjectile>().damage = damage;
        projectile.transform.position = gameObject.transform.position;

        Vector3 offset = Player.PlayerTransform.position - gameObject.transform.position;
        float rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg + 90;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));

        projectile.SetActive(true);
        projectile.GetComponent<Rigidbody2D>().velocity = projectileSpeed * offset;
    }

    void StartCharging()
    {
        rb2d.velocity = Vector2.zero;
        isPreparingForShot = true;
        animator.SetBool("Charging", true);
    }

    void StopCharging()
    {
        isPreparingForShot = false;
        animator.SetBool("Charging", false);
    }

    bool IsVisible()
    {
        Vector3 pos = mainCam.WorldToViewportPoint(transform.position);
        return pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1;
    }
}
