using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(BoxCollider2D))]
[RequireComponent (typeof(ProjectileDestroy))]
[RequireComponent (typeof(Rigidbody2D))]
public class BasicEnemyProjectile : MonoBehaviour {
    public int damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().InflictDamage(damage);
        }

        if (other.tag == "Player" || other.tag == "Wall" || other.tag == "Obstacle")
        {
            GetComponent<ProjectileDestroy>().Destroy();
        }
    }
}
