using UnityEngine;
using System.Collections.Generic;

public class ChargeBlastProperties : BasicPlayerProjectile
{
    public int ImpactDamage { get;  set; }

    // int defaults to 0.
    public int ChargeLevel { get; set; }
    public bool Fired { get; set; }

    private ProjectileDestroy projectileDestroy;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int enemyLayerMask = 1 << 9;

    private List<int> damageLevels = new List<int> { 3, 5, 7 };
    private List<int> explosionDamageLevels = new List<int> { 1, 2, 3 };
    private List<float> explosionRadiusLevels = new List<float> { .6f, .8f, .9f };

    void Awake()
    {
        projectileDestroy = GetComponent<ProjectileDestroy>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void DestroyEnergyOrb()
    {
        animator.SetTrigger("Destroyed");
        projectileDestroy.Destroy();
    }

    void RevealOrb()
    {
        spriteRenderer.enabled = true;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!Fired)
        {
            return;
        }

        if (isEnemy(other))
        {
            other.GetComponent<EnemyHealth>().InflictDamage(damageLevels[ChargeLevel]);
        }

        if (isObstacle(other))
        {
            if (ChargeLevel == 0)
            {
                projectileDestroy.Destroy();
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetTrigger("Impact");
                DoExplosion(explosionRadiusLevels[ChargeLevel], explosionDamageLevels[ChargeLevel]);
            }

            Fired = false;
        }
    }

    void DoExplosion(float radius, int damage)
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.transform.position, radius, enemyLayerMask);
        foreach (Collider2D collider in nearbyEnemies)
        {
            collider.GetComponent<EnemyHealth>().InflictDamage(damage);
        }
    }
}
