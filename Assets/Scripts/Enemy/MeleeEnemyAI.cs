using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeEnemyAI : MonoBehaviour {
    public static HashSet<MeleeEnemyAI> meleeEnemies = new HashSet<MeleeEnemyAI>();

    public int chargeDistance;
    public float attackDistance;
    public float speed;
    public float pathFindingRate;
    public float chaseTime;
    public float nearbyEnemyRadius;
    public Vector2 target;
    public Vector2 token;

    public bool targetIsAssigned = false;
    public bool shouldWait = true;

    public float squaredAttackDistance;
    public BoxCollider2D boxCollider2d;
    public FiniteStateMachine<MeleeEnemyAI> fsm;
    public bool chasing = false;

    private Rigidbody2D rb2d;
    private bool isFirstFrame = true;
    private float lastPathfindTime = 0;

	void Awake () {
        squaredAttackDistance = attackDistance * attackDistance;

        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        fsm = new FiniteStateMachine<MeleeEnemyAI>(this, MeleeEnemyIdleState.Instance);

        meleeEnemies.Add(this);
	}

    void OnDisable()
    {
        meleeEnemies.Remove(this);
    }
	
	void Update () {
        if (isFirstFrame)
        {
            isFirstFrame = false;
            return;
        }

        fsm.Update();
	}

    public void Charge(Vector2 target, float colliderSizeMultiplierX, float colliderSizeMultiplierY, float boxCastDistance)
    {
        if (EnemyUtil.CanSee(transform.position, target) &&
                    EnemyUtil.PathIsNotBlocked(boxCollider2d, transform.position, target, colliderSizeMultiplierX,
                    colliderSizeMultiplierY, boxCastDistance))
        {
            CalculateVelocity(target);
        }
        else
        {
            ExecuteAStar(target);
        }
    }

    public void ExecuteAStar(Vector3 target)
    {
        if (Time.time > lastPathfindTime + pathFindingRate)
        {
            lastPathfindTime = Time.time;
            List<AStar.Node> list = AStar.calculatePath(AStar.positionToArrayIndices(transform.position),
                AStar.positionToArrayIndices(target));

            if (list.Count > 1)
            {
                CalculateVelocity(AStar.arrayIndicesToPosition(list[1].point));
            }
        }
    }

    public void CalculateVelocity(Vector2 target)
    {
        Vector2 pullVector = new Vector2(target.x - transform.position.x,
            target.y - transform.position.y).normalized * speed;
        Vector2 pushVector = Vector2.zero;

        // Find all nearby enemies
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, nearbyEnemyRadius, LayerMasks.ENEMY_LAYER_MASK);
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

        rb2d.velocity = pullVector.normalized * speed;
    }

    public void StopMovement()
    {
        rb2d.velocity = Vector2.zero;
    }

    void DeactivateChase()
    {
        chasing = false;
    }
}
