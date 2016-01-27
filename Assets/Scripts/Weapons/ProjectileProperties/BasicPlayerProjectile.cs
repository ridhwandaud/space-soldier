using UnityEngine;
using System.Collections;

public class BasicPlayerProjectile : MonoBehaviour
{
    public int damage;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemy(other))
        {
            other.GetComponent<EnemyHealth>().InflictDamage(damage);
        }
        if (isObstacle(other))
        {
            GetComponent<ProjectileDestroy>().Destroy();
            Destructible destructible = other.GetComponent<Destructible>();
            if (destructible)
            {
                destructible.InflictDamage(damage);
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
