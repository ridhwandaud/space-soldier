using UnityEngine;
using System.Collections;

public class EnergyOrbProperties : BasicPlayerProjectile
{
    public int explosionDamage = 3;
    public float explosionRadius = .6f;

    private Animator animator;
    private Rigidbody2D rb;
    private ProjectileDestroy projectileDestroy;
    private int enemyLayerMask = 1 << 9;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        projectileDestroy = GetComponent<ProjectileDestroy>();
    }

    void DestroyEnergyOrb()
    {
        animator.SetBool("exploding", false);
        projectileDestroy.Destroy();
    }

    void DoExplosion()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.transform.position, explosionRadius, enemyLayerMask);
        foreach(Collider2D collider in nearbyEnemies) {
            collider.GetComponent<EnemyHealth>().InflictDamage(explosionDamage);
        }
    }
}
