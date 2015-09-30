using UnityEngine;
using System.Collections;

public class BasicPlayerProjectile : MonoBehaviour
{
    public int damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyHealth>().InflictDamage(damage);
        }
        if (other.tag == "Enemy" || other.tag == "Wall")
        {
            GetComponent<ProjectileDestroy>().Destroy();
        }
    }
}
