using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour {

    private Rigidbody2D rb2d;
    private EnemyAI enemyAI;

	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
	}

    void Update()
    {
        if (Time.time > enemyAI.KnockbackEndTime && enemyAI.KnockbackInProgress)
        {
            rb2d.velocity = Vector2.zero;
            enemyAI.KnockbackInProgress = false;
        }
    }

    public void KnockBack (Vector3 projectileVelocity)
    {
        rb2d.velocity = projectileVelocity.normalized * GameSettings.KnockbackVelocity;
        enemyAI.KnockbackEndTime = Time.time + GameSettings.KnockbackDuration;
        enemyAI.KnockbackInProgress = true;
    }
}
