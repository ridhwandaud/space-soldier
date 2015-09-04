using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeEnemyAI : MonoBehaviour {
    public static HashSet<MeleeEnemyAI> meleeEnemies = new HashSet<MeleeEnemyAI>();

    public int chargeDistance = 12;
    public float attackDistance = 5;
    public float speed = 2f;
    public float pathFindingRate = 2f;
    public float chaseTime = 3f;
    public bool isWithinAttackingRange = false;
    public Vector2 target;
    public Vector2 token;
    public bool targetIsAssigned = false;
    public bool shouldWait = true;

    // Temporary variables for debugging.
    public string state;
    public float debugDistance = 0;

    private int wallLayerMask = 1 << 8;

    private GameObject player;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2d;

    public bool chasing = false;
    private bool isFirstFrame = true;
    private float lastPathfindTime = 0;

	void Awake () {
        // Can't set this in inspector because these are generated via prefabs.
        player = GameObject.Find("Soldier");
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        meleeEnemies.Add(this);
	}

    void OnDisable()
    {
        meleeEnemies.Remove(this);
    }
	
	void Update () {
        // TODO: Remove this temporary line.
        debugDistance = (player.transform.position - transform.position).sqrMagnitude;

        if (isFirstFrame)
        {
            isFirstFrame = false;
            return;
        }

        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = player.transform.position;

        float distanceFromPlayer = Vector3.Distance(playerPosition, enemyPosition);
        isWithinAttackingRange = distanceFromPlayer <= attackDistance;

        if (isWithinAttackingRange)
        {
            if (!targetIsAssigned)
            {
                if (shouldWait)
                {
                    state = "in range no token";
                    rb2d.velocity = Vector2.zero;
                }
                else if (EnemyUtil.CanSee(transform.position, player.transform.position) &&
                EnemyUtil.PathIsNotBlocked(boxCollider2d, transform.position, player.transform.position, .8f, .8f))
                {
                    //state = "unblocked target token";
                    rb2d.velocity = CalculateVelocity(player.transform.position);
                }
                else
                {
                    //state = "astar target token";
                    ExecuteAStar(player.transform.position);
                }
                return;
            }


            if (Vector3.Distance(enemyPosition, target) <= .2)
            {
                state = "within .1";
                rb2d.velocity = Vector2.zero;
            }
            else if (EnemyUtil.CanSee(transform.position, player.transform.position) && 
                EnemyUtil.PathIsNotBlocked(boxCollider2d, transform.position, target, .8f, .8f))
            {
                state = "unblocked target token";
                rb2d.velocity = CalculateVelocity(target);
            }
            else
            {
                state = "astar target token";
                ExecuteAStar(target);
            }

            return;
        }

        targetIsAssigned = false;
        shouldWait = false;

        if (EnemyUtil.CanSee(transform.position, player.transform.position))
        {
            // Just realized this is not quite true because the player might not be in range, but functionally the result is the
            // same.
            chasing = true;
            CancelInvoke("DeactivateChase");
            if (distanceFromPlayer <= chargeDistance)
            {
                if(EnemyUtil.PathIsNotBlocked(boxCollider2d, transform.position, player.transform.position)) {
                    state = "charging";
                    rb2d.velocity = CalculateVelocity(player.transform.position);
                }
                else
                {
                    state = "charging astar";
                    ExecuteAStar(player.transform.position);
                }
            }
            else
            {
                state = "far, not chasing";
                rb2d.velocity = CalculateVelocity(enemyPosition);
            }
        }
        else
        {
            if (chasing)
            {
                // Should probably also deactivate this if the player isn't close enough... maybe CanSeePlayer can include
                // a vision distance.
                Invoke("DeactivateChase", chaseTime);
            }
            if (distanceFromPlayer <= chargeDistance && chasing)
            {
                state = "astar out of sight";
                ExecuteAStar(player.transform.position);
            }
            else
            {
                state = "cant see player, not chasing";
                chasing = false; // is this necessary?
                rb2d.velocity = CalculateVelocity(enemyPosition);
            }
        }
	}

    void ExecuteAStar(Vector3 target)
    {
        if (Time.time > lastPathfindTime + pathFindingRate)
        {
            lastPathfindTime = Time.time;
            List<AStar.Node> list = AStar.calculatePath(AStar.positionToArrayIndices(transform.position),
                AStar.positionToArrayIndices(target));

            if (list.Count > 1)
            {
                rb2d.velocity = CalculateVelocity(AStar.arrayIndicesToPosition(list[1].point));
            }
        }
    }

    Vector2 CalculateVelocity(Vector2 target)
    {
        return new Vector2(target.x - transform.position.x, target.y - transform.position.y).normalized * speed;
    }

    void DeactivateChase()
    {
        chasing = false;
    }
}
