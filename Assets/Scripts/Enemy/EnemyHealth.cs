using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

    public int health = 20;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player Projectile")
        {
            ProjectileProperties projectileProperties = other.GetComponent<ProjectileProperties>();

            other.GetComponent<ProjectileDestroy>().Destroy();
            InflictDamage(projectileProperties.Damage);
        }
    }

    public void InflictDamage(int damagePoints)
    {
        health -= damagePoints;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
