using UnityEngine;
using System.Collections;

public class EnemyDeath : MonoBehaviour {
    public int experiencePoints;

    private PlayerExperience playerExperience;
    private bool destroyed = false;

    void Awake()
    {
        playerExperience = GameObject.Find("Soldier").GetComponent<PlayerExperience>();
    }

    public void KillEnemy()
    {
        // Destroy doesn't actually take effect until after the update loop, so I added this condition to prevent explosive damage (and other forms
        // of secondary damage) from doubling the experience points earned.
        if (!destroyed)
        {
            playerExperience.IncrementExperience(experiencePoints);
            Destroy(gameObject);
            destroyed = true;
        }
    }
}
