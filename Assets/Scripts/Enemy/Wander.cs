using UnityEngine;
using System.Collections;

public class Wander : MonoBehaviour {

    public Vector2 walkTimeRange;
    public Vector2 waitTimeRange;
    public float wanderSpeed;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private float nextWanderTime;
    private EnemyAI enemyAI;
    private Vector2 colliderSize;
    private bool shouldMove = true;
    private bool wasInvisibleOnLastIteration = false;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        nextWanderTime = Time.time + Random.Range(0f, 3f);
        colliderSize = GetComponent<BoxCollider2D>().size;

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}

    public void DoWander()
    {
        if (!isVisible())
        {
            wasInvisibleOnLastIteration = true;
            return;
        }

        if (wasInvisibleOnLastIteration)
        {
            wasInvisibleOnLastIteration = false;
            nextWanderTime = Time.time + Random.Range(0f, 3f);
            return;
        }

        if (shouldWander())
        {
            if (shouldMove && Random.Range(0f, 3f) > 1)
            {
                shouldMove = false;
                Vector2 dir = EnemyUtil.CalculateUnblockedDirection(transform.position, colliderSize, 1f, true);
                rb.velocity = EnemyUtil.CalculateVelocityFromPullVector(transform, dir, wanderSpeed);
                nextWanderTime = Time.time + Random.Range(walkTimeRange.x, walkTimeRange.y);
            }
            else
            {
                shouldMove = true;
                rb.velocity = EnemyUtil.CalculateVelocity(transform, transform.position, wanderSpeed);
                nextWanderTime = Time.time + Random.Range(waitTimeRange.x, waitTimeRange.y);
            }
        }
    }

    bool isVisible()
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        return viewPos.x >= -3 && viewPos.x <= 4 && viewPos.y >= -3 && viewPos.y <= 4;
    }

    bool shouldWander()
    {
        return !enemyAI.chasing && Time.time > nextWanderTime;
    }
}
