using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeEnemyAI : EnemyAI {
    public static HashSet<MeleeEnemyAI> meleeEnemies = new HashSet<MeleeEnemyAI>();

    public int chargeDistance;
    public float attackDistance;
    public float pathFindingRate = 1f;
    public float chaseTime;
    public Vector2 target;
    public Vector2 token;

    public bool targetIsAssigned = false;
    public bool shouldWait = true;

    public float squaredAttackDistance;
    public BoxCollider2D boxCollider2d;
    public FiniteStateMachine<MeleeEnemyAI> fsm;
    public Animator animator;

    private Rigidbody2D rb2d;
    private Wander wanderScript;
    private float lastPathfindTime = 0;

	void Awake () {
        squaredAttackDistance = attackDistance * attackDistance;

        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        wanderScript = GetComponent<Wander>();
        fsm = new FiniteStateMachine<MeleeEnemyAI>(this, MeleeEnemyIdleState.Instance);

        meleeEnemies.Add(this);
	}

    void OnDisable()
    {
        meleeEnemies.Remove(this);
    }
	
	void Update () {
        if (!GameState.WallCollidersInitialized || KnockbackInProgress)
        {
            return;
        }

        ChaseIfNecessary();
        fsm.Update();
	}

    public void Charge(Vector2 target, float colliderSizeMultiplierX, float colliderSizeMultiplierY,
        float boxCastDistance)
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
        EnemyUtil.ExecuteAStar(transform, target, rb2d, ref lastPathfindTime, pathFindingRate, speed);
    }

    public void CalculateVelocity(Vector2 target)
    {
        rb2d.velocity = EnemyUtil.CalculateVelocity(transform, target, speed);
    }

    public void StopMovement()
    {
        rb2d.velocity = Vector2.zero;
    }

    void DeactivateChase()
    {
        chasing = false;
    }

    public void Wander()
    {
        wanderScript.DoWander();
    }
}
