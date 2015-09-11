using UnityEngine;
using System.Collections;

public class EnemyDeath : MonoBehaviour {
    public int experiencePoints;

    private bool destroyed = false;

    public void KillEnemy()
    {
        // Destroy doesn't actually take effect until after the update loop, so I added this condition to prevent explosive damage (and other forms
        // of secondary damage) from doubling the experience points earned.
        if (!destroyed)
        {
            Player.PlayerExperience.IncrementExperience(experiencePoints);
            Destroy(gameObject);
            destroyed = true;
            GameState.NumEnemiesRemaining--;
            if (GameState.NumEnemiesRemaining == 0)
            {
                createPortal();
            }
        }
    }

    void createPortal()
    {
        GameObject portal = GameObject.Find("Portal");
        portal.transform.position = transform.position;
        portal.GetComponent<SpriteRenderer>().enabled = true;
        portal.GetComponent<BoxCollider2D>().enabled = true;
    }
}
