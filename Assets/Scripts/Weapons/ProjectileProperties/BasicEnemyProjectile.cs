using UnityEngine;
using System.Collections;

public class BasicEnemyProjectile : MonoBehaviour {
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().InflictDamage(damage);
        }

        if (other.tag == "Player" || other.tag == "Wall")
        {
            GetComponent<ProjectileDestroy>().Destroy();
        }
    }
}
