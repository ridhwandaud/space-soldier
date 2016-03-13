using UnityEngine;
using System.Collections;

public class BasicPlayerProjectile : MonoBehaviour
{
    protected Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public int Damage
    {
        get; set;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemy(other))
        {
            other.GetComponent<EnemyHealth>().InflictDamage(Damage);
            other.GetComponent<Knockback>().KnockBack(rb2d.velocity);
        }
        if (isObstacle(other))
        {
            GetComponent<ProjectileDestroy>().Destroy();
            Destructible destructible = other.GetComponent<Destructible>();
            if (destructible)
            {
                destructible.InflictDamage(Damage);
            }
        }
    }

    protected bool isEnemy(Collider2D other)
    {
        return other.tag == "Enemy";
    }

    protected bool isObstacle(Collider2D other)
    {
        return other.tag == "Enemy" || other.tag == "Wall" || other.tag == "Obstacle";
    }
}
