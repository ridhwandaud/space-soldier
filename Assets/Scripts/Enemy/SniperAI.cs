using UnityEngine;

public class SniperAI : EnemyAI {

    public float timeBetweenShots;
    public int damage;
    public float aimStoppingDelay; // Amount of time to wait after player is out of sight before aimer goes away.

    private float nextFiringTime = 0;
    private bool isAiming = false;
    private Animator animator;
    private Camera mainCam;
    private Wander wanderScript;
    private Rigidbody2D rb2d;
    private LineRenderer lineRenderer;

    // TODO: Refactor.
    private float projectileSpeed = 10f;
    private StackPool projectilePool;
    private bool sniperWasHit = false;
    private bool firing = false;
    private Vector3 lockedPosition;

	void Awake () {
        projectilePool = GameObject.Find("FireballPool").GetComponent<StackPool>();
        animator = GetComponent<Animator>();
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        wanderScript = GetComponent<Wander>();
        rb2d = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }
	
	void Update ()
    {
        if (KnockbackInProgress || GameSettings.PauseAllEnemies)
        {
            return;
        }

        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = Player.PlayerTransform.position;

        if(sniperWasHit)
        {
            sniperWasHit = false;
            StopAiming();
            HideBeam();
            return;
        }

        if (EnemyUtil.CanSee(transform.position, playerPosition))
        {
            if (EnemyUtil.IsOnScreen(transform.position) && Time.time > nextFiringTime && !isAiming)
            {
                chasing = true;
                StartAiming();
            }
        } else if (isAiming)
        {
            // TODO: Add some time before the next wander so that the sniper doesn't always move the second you get out of its view.
            chasing = false;
            Invoke("BecomeIdle", aimStoppingDelay);
        }

        if (firing)
        {
            Aim(lockedPosition);
        }
        else if (isAiming)
        {
            Aim(Player.PlayerTransform.position);
        } else
        {
            wanderScript.DoWander();
        }
	}

    void BecomeIdle()
    {
        HideBeam();
        StopAiming();
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

    void Fire()
    {
        Invoke("ActuallyFire", .5f);
        firing = true;
        lockedPosition = Player.PlayerTransform.position;

        StopAiming();
    }

    void ActuallyFire()
    {
        nextFiringTime = Time.time + timeBetweenShots;

        GameObject projectile = projectilePool.Pop();
        projectile.GetComponent<BasicEnemyProjectile>().damage = damage;
        projectile.transform.position = gameObject.transform.position;

        Vector3 offset = lockedPosition - gameObject.transform.position;
        float rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg + 90;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));

        projectile.SetActive(true);
        projectile.GetComponent<Rigidbody2D>().velocity = projectileSpeed * offset;
        HideBeam();

        firing = false;
    }

    void StartAiming()
    {
        CancelInvoke("BecomeIdle");
        rb2d.velocity = Vector2.zero;
        isAiming = true;
        animator.SetBool("Charging", true);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, Player.PlayerTransform.position);
    }

    void Aim(Vector2 target)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target - (Vector2)transform.position, 20, LayerMasks.SniperAimLayerMask);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hit.point);
    }

    void StopAiming()
    {
        isAiming = false;
        animator.SetBool("Charging", false);
    }

    void HideBeam()
    {
        lineRenderer.enabled = false;
    }
}
