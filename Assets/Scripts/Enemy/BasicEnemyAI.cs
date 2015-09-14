using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class BasicEnemyAI : EnemyAI {

    public int firingDistance;
    public int chargeDistance;
    public float speed;
    public float nearbyEnemyRadius;
    public float pathFindingRate;
    public float chaseTime;
    public float attackDelay;

    public bool readyToAttack = false;
    public bool attackInvoked = false;

    private Wander wanderScript;
    private BasicEnemyFire fireScript;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2d;

    private float lastPathfindTime = 0;

    void Awake()
    {
        wanderScript = GetComponent<Wander>();
        fireScript = GetComponent<BasicEnemyFire>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
    }
	
	void Update () {
        if (!LoadLevel.WALL_COLLIDERS_INITIALIZED)
        {
            return;
        }
        base.Update();

        Vector2 enemyPosition = transform.position;
        Vector2 playerPosition = Player.PlayerTransform.position;

        float distanceFromPlayer = Vector3.Distance(playerPosition, enemyPosition);

        if (EnemyUtil.CanSee(transform.position, playerPosition) && distanceFromPlayer <= chargeDistance)
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
            else
            {
                readyToAttack = false;
                Charge();
            }
        }
        else {
            readyToAttack = false;
            if (chasing)
            {
                Invoke("DeactivateChase", chaseTime);
                Charge();
            }
            else
            {
                rb2d.velocity = CalculateVelocity(enemyPosition);
            }
        } 
	}

    public void Charge()
    {
        if (EnemyUtil.CanSee(transform.position, Player.PlayerTransform.position) &&
            EnemyUtil.PathIsNotBlocked(boxCollider2d, transform.position, Player.PlayerTransform.position))
        {
            rb2d.velocity = CalculateVelocity(Player.PlayerTransform.position);
        }
        else
        {
            ExecuteAStar(transform.position, Player.PlayerTransform.position);
        }
    }

    void ActivateAttack()
    {
        readyToAttack = true;
        attackInvoked = false;
    }

    void StartFiring()
    {
        rb2d.velocity = CalculateVelocity(transform.position);
        fireScript.Fire();
    }

    void ExecuteAStar(Vector2 enemyPosition, Vector2 playerPosition)
    {
        EnemyUtil.ExecuteAStar(transform, playerPosition, rb2d, ref lastPathfindTime, pathFindingRate, speed, nearbyEnemyRadius);
    }

    public Vector2 CalculateVelocity(Vector2 target)
    {
        return EnemyUtil.CalculateVelocity(transform, target, speed, nearbyEnemyRadius);
    }
}
