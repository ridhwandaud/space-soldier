using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class BasicEnemyAI : MonoBehaviour {

    public int firingDistance = 7;
    public int chargeDistance = 12;
    public float speed = 2f;
    public float nearbyEnemyRadius = .01f;
    public float pathFindingRate = 2f;
    public float chaseTime = 3f;
    public float attackDelay = .5f;

    private GameObject player;
    private Wander wanderScript;
    private BasicEnemyFire fireScript;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2d;

    private int wallLayerMask = 1 << 8; // Layer 8 is the wall layer.
    private int enemyLayerMask = 1 << 9;
    private float lastPathfindTime = 0;
    private bool chasing = false;
    private bool readyToAttack = false;
    private bool attackInvoked = false;

    private bool isFirstFrame = true;

    void Awake()
    {
        player = GameObject.Find("Soldier");
        wanderScript = GetComponent<Wander>();
        fireScript = GetComponent<BasicEnemyFire>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
    }
	
	void Update () {
        if (isFirstFrame)
        {
            isFirstFrame = false;
            return;
        }

        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = player.transform.position;

        float distanceFromPlayer = Vector3.Distance(playerPosition, enemyPosition);

        if (EnemyUtil.CanSeePlayer(transform, player.transform))
        {
            chasing = true;
            CancelInvoke("DeactivateChase");
            if (distanceFromPlayer <= firingDistance)
            {
                if (readyToAttack)
                {
                    rb2d.velocity = CalculateVelocity(enemyPosition);
                    fireScript.Fire();
                }
                else if (!attackInvoked)
                {
                    attackInvoked = true;
                    Invoke("ActivateAttack", attackDelay);
                }
            }
            else if (distanceFromPlayer <= chargeDistance)
            {
                readyToAttack = false;
                if (EnemyUtil.PathToPlayerIsNotBlocked(boxCollider2d, transform, player.transform))
                {
                    rb2d.velocity = CalculateVelocity(player.transform.position);
                }
                else
                {
                    ExecuteAStar(enemyPosition, playerPosition);
                }
            }
            else
            {
                // Weird "bug" caused by this logic - if the enemy can see the player and the player goes out of range, the enemy immediately
                // stops moving. But if the enemy CAN'T see the player, then it will continue chasing until the chaseTime timer
                // runs out. This behavior makes little sense logically but probably isn't even noticeable by players.
                readyToAttack = false;
                rb2d.velocity = CalculateVelocity(enemyPosition);
            }
        }
        else {
            readyToAttack = false;
            if (chasing)
            {
                Invoke("DeactivateChase", chaseTime);
            }

            if (distanceFromPlayer <= chargeDistance && chasing)
            {
                ExecuteAStar(enemyPosition, playerPosition);
            }
            else
            {
                chasing = false;
                rb2d.velocity = CalculateVelocity(enemyPosition);
            }
        } 
	}

    void ActivateAttack()
    {
        readyToAttack = true;
        attackInvoked = false;
    }

    void DeactivateChase()
    {
        chasing = false;
    }

    void StartFiring()
    {
        rb2d.velocity = CalculateVelocity(transform.position);
        fireScript.Fire();
    }

    void ExecuteAStar(Vector2 enemyPosition, Vector2 playerPosition)
    {
        if (Time.time > lastPathfindTime + pathFindingRate)
        {
            lastPathfindTime = Time.time;
            List<AStar.Node> list = AStar.calculatePath(AStar.positionToArrayIndices(enemyPosition),
                AStar.positionToArrayIndices(playerPosition));

            if (list.Count > 1)
            {
                rb2d.velocity = CalculateVelocity(AStar.arrayIndicesToPosition(list[1].point));
            }
        }
    }

    Vector2 CalculateVelocity(Vector2 target)
    {
        Vector2 pullVector = new Vector2(target.x - transform.position.x,
            target.y - transform.position.y).normalized * speed;
        Vector2 pushVector = Vector2.zero;

        // Find all nearby enemies
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, nearbyEnemyRadius, enemyLayerMask);
        int contenders = 0;

        for (int i = 0; i < nearbyEnemies.Length; i++)
        {
            if (nearbyEnemies[i].transform == transform)
            {
                continue;
            }

            Vector2 push = transform.position - nearbyEnemies[i].transform.position;
            pushVector += push / push.sqrMagnitude;

            contenders++;
        }


        pullVector *= Mathf.Max(1, 4 * contenders);
        pullVector += pushVector;

        return pullVector.normalized * speed;
    }
}
