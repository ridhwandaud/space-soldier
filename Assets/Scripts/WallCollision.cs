using UnityEngine;
using System.Collections;

public class WallCollision : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player Projectile")
        {
            other.GetComponent<ProjectileDestroy>().Destroy();
        }
    }
}
