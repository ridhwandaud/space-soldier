using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour {

    private Rigidbody2D rb2d;
    private EnemyAI enemyAI;

	// Use this for initialization
	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
	}

    public void KnockBack (Vector3 projectileVelocity)
    {
        rb2d.velocity = projectileVelocity.normalized * GameSettings.KnockbackVelocity;
        enemyAI.KnockbackEndTime = Time.time + GameSettings.KnockbackDuration;
    }
}
