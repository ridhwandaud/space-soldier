using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class BasicEnemyAI : MonoBehaviour {

    public int attackDistance = 7;
    public int chargeDistance = 12;
    public float speed = 2f;
    public float nearbyEnemyRadius = .01f;
    public float chaseTimeSeconds = 3f;
    public float pathFindingRate = 1f;

    private GameObject player;
    private Wander wanderScript;
    private BasicEnemyFire fireScript;
    private Rigidbody2D rb2d;

    private int wallLayerMask = 1 << 8; // Layer 8 is the wall layer.
    private int enemyLayerMask = 1 << 9;
    private float lastPathfindTime = 0;

    void Awake()
    {
        player = GameObject.Find("Soldier");
        wanderScript = GetComponent<Wander>();
        fireScript = GetComponent<BasicEnemyFire>();
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	void Update () {
        Vector2 enemyPosition = gameObject.transform.position;
        Vector2 playerPosition = player.transform.position;

        float distanceFromPlayer = Vector3.Distance(playerPosition, enemyPosition);

        //if (CanSeePlayer())
        //{
            if (distanceFromPlayer <= attackDistance)
            {
                rb2d.velocity = CalculateVelocity(enemyPosition);
                fireScript.Fire();
            }
            else if (distanceFromPlayer <= chargeDistance)
            {
                // Do A*

                if (Time.time > lastPathfindTime + pathFindingRate)
                {
                    lastPathfindTime = Time.time;
                    List<AStar.Node> list = AStar.calculatePath(new Int2((int)enemyPosition.x / 2, (int)enemyPosition.y / 2),
                        new Int2((int)playerPosition.x / 2, (int)playerPosition.y / 2));
                    print(list.Count);
                    if (hasReachedNode(list[0]))
                    {
                        rb2d.velocity = CalculateVelocity(new Vector2(list[1].point.x * 2f - 1f, list[1].point.y * 2f - 1f));
                    }
                    else
                    {
                        rb2d.velocity = CalculateVelocity(new Vector2(list[0].point.x * 2f - 1f, list[0].point.y * 2f - 1f));
                    }
                }
                //rb2d.velocity = CalculateVelocity(player.transform.position);
            }
            else
            {
                rb2d.velocity = CalculateVelocity(enemyPosition);
            }
        //}
        //else
        //{
            //rb2d.velocity = CalculateVelocity(enemyPosition);
        //}
	}

    bool hasReachedNode(AStar.Node node)
    {
        return (gameObject.transform.position - new Vector3(node.point.x * 2f - 1f, node.point.y * 2f - 1f, 0)).sqrMagnitude < .8;
    }

    Vector2 CalculateVelocity(Vector2 target)
    {
        Vector2 pullVector = new Vector2(target.x - gameObject.transform.position.x,
            target.y - gameObject.transform.position.y).normalized * speed;
        Vector2 pushVector = Vector2.zero;

        // Find all nearby enemies
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.transform.position, nearbyEnemyRadius, enemyLayerMask);
        int contenders = 0;

        for (int i = 0; i < nearbyEnemies.Length; i++)
        {
            if (nearbyEnemies[i].transform == gameObject.transform)
            {
                continue;
            }

            Vector2 push = gameObject.transform.position - nearbyEnemies[i].transform.position;
            pushVector += push / push.sqrMagnitude;

            contenders++;
        }

        pullVector *= Mathf.Max(1, contenders);
        pullVector += pushVector;

        return pullVector.normalized * speed;
    }

    bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position, wallLayerMask);
        
        return hit.transform == null;
    }
}
