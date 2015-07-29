using UnityEngine;
using System.Collections;

public class EnemyDeath : MonoBehaviour {
    public int experiencePoints;

    // To increment experience?
    private GameObject player;

    void Awake()
    {
        player = GameObject.Find("Soldier");
    }

    public void KillEnemy()
    {
        Destroy(gameObject);
    }
}
