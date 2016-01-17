using UnityEngine;

public class KnightAI : EnemyAI {
    // 1 Attack = a volley of shots.
    public float timeBetweenAttacks;
    public int numShotsPerVolley;
    public int damage;

    public Sprite attackingSprite;
    public Sprite guardingSprite;

    private bool attacking = false;
    private float nextAttackTime = 0;
    private int shotsFiredThisTurn = 0;

    private EnemyHealth enemyHealth;
    private SpriteRenderer spriteRenderer;
    private BasicEnemyFire firingScript;

	void Awake () {
        enemyHealth = GetComponent<EnemyHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        firingScript = GetComponent<BasicEnemyFire>();

        ActivateGuard();
	}
	
	// Update is called once per frame
	void Update () {
        if (!LoadLevel.WallCollidersInitialized)
        {
            return;
        }

        ChaseIfNecessary();

        if (EnemyUtil.CanSee(transform.position, Player.PlayerTransform.position) && !attacking && Time.time > nextAttackTime)
        {
            shotsFiredThisTurn = 0;
            ActivateAttack();
        }

        if (attacking)
        {
            shotsFiredThisTurn += firingScript.Fire();

            if (shotsFiredThisTurn >= numShotsPerVolley)
            {
                ActivateGuard();
                nextAttackTime = Time.time + timeBetweenAttacks;
            }
        }
	}

    void ActivateAttack()
    {
        attacking = true;
        enemyHealth.guarded = false;
        spriteRenderer.sprite = attackingSprite;
    }

    void ActivateGuard()
    {
        attacking = false;
        enemyHealth.guarded = true;
        spriteRenderer.sprite = guardingSprite;
    }
}
