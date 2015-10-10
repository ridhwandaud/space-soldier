using UnityEngine;
using System.Collections;

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

    private Rigidbody2D rb2d;
    private EnemyHealth enemyHealth;
    private SpriteRenderer renderer;
    private BasicEnemyFire firingScript;

	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();
        renderer = GetComponent<SpriteRenderer>();
        firingScript = GetComponent<BasicEnemyFire>();

        ActivateGuard();
	}
	
	// Update is called once per frame
	void Update () {
        if (!LoadLevel.WALL_COLLIDERS_INITIALIZED)
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
        enemyHealth.invincible = false;
        renderer.sprite = attackingSprite;
    }

    void ActivateGuard()
    {
        attacking = false;
        enemyHealth.invincible = true;
        renderer.sprite = guardingSprite;
    }
}
