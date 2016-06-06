using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {
    public bool guarded = false;

    private EnemyAI enemyAI;
    private EnemyDeath enemyDeath;
    private Animator animator;

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyDeath = GetComponent<EnemyDeath>();
        animator = GetComponent<Animator>();
    }

    public int health = 20;

    public void InflictDamage(int damagePoints)
    {
        if (guarded)
        {
            return;
        }

        health -= damagePoints;
        if (enemyAI)
        {
            enemyAI.chasing = true;
        }

        if (health <= 0)
        {
            enemyDeath.KillEnemy();
        } else
        {
            animator.SetBool("Hit", true);
        }
    }

    public void HitDone()
    {
        animator.SetBool("Hit", false);
    }
}
