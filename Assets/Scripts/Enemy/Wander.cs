using UnityEngine;
using System.Collections;

public class Wander : MonoBehaviour {

    public Vector2 timeBetweenWanders;
    public int movementVariationDegrees;
    public float wanderDistance;

    private Rigidbody2D rb;
    private float nextWanderTime;
    private EnemyAI enemyAI;
    private Vector2 colliderSize;
    private bool shouldMove = true;
    private float squaredWanderDistance;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        nextWanderTime = Time.time + Random.Range(0f, 3f);
        colliderSize = GetComponent<BoxCollider2D>().size;
        squaredWanderDistance = wanderDistance * wanderDistance;
	}

    public void DoWander()
    {
        if (shouldWander())
        {
            if (shouldMove && Vector2.SqrMagnitude(Player.PlayerTransform.position - transform.position)
                <= squaredWanderDistance && Random.Range(0, 3) > 1)
            {
                shouldMove = false;
                Vector2 dir = EnemyUtil.CalculateUnblockedDirection(movementVariationDegrees, transform.position, colliderSize, 1f);
                rb.velocity = EnemyUtil.CalculateVelocityFromPullVector(transform, dir, enemyAI.speed, enemyAI.nearbyEnemyRadius);
            }
            else
            {
                shouldMove = true;
                rb.velocity = EnemyUtil.CalculateVelocity(transform, transform.position, enemyAI.speed, enemyAI.nearbyEnemyRadius);
            }

            nextWanderTime = Time.time + Random.Range(timeBetweenWanders.x, timeBetweenWanders.y);
        }
    }

    bool shouldWander()
    {
        return !enemyAI.chasing && Time.time > nextWanderTime;
    }
}
