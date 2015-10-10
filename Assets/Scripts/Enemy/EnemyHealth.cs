using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
    public bool invincible = false;

    private EnemyAI enemyAI;
    private EnemyDeath enemyDeath;

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyDeath = GetComponent<EnemyDeath>();
    }

    public int health = 20;

    public void InflictDamage(int damagePoints)
    {
        if (invincible)
        {
            return;
        }

        health -= damagePoints;
        enemyAI.chasing = true;

        if (health <= 0)
        {
            enemyDeath.KillEnemy();
        }
    }
}
