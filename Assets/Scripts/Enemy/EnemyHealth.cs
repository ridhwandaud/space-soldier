using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

    public int health = 20;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player Bullet")
        {
            health--;
            other.GetComponent<BulletDestroy>().Destroy();

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
