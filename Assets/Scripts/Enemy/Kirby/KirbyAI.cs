using UnityEngine;

public class KirbyAI : EnemyAI {
    public float guardRange;
    public float seekRange;
    public FiniteStateMachine<KirbyAI> fsm;
    public LineRenderer lineRenderer;
    public EnemyAI guardedEnemy;
    public float hideDistance;
    public float actualSpeed;

    // TODO: Consider moving these into the parent class...
    public Rigidbody2D rb2d;
    public BoxCollider2D boxCollider2D;

    private float squaredGuardRange;

    public float lastPathfindTime = 0;
    public float pathFindingRate;

    void Awake () {
        squaredGuardRange = guardRange * guardRange;
        lineRenderer = GetComponent<LineRenderer>();
        fsm = new FiniteStateMachine<KirbyAI>(this, KirbySeekingState.Instance);
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        actualSpeed = speed;
	}
	
	void Update () {
        if (Time.time < KnockbackEndTime || GameSettings.PauseAllEnemies)
        {
            return;
        }

        fsm.Update();
	}

    public EnemyAI GetClosestSeekableEnemy()
    {
        // Should this happen at an interval, rather than every update loop? How expensive is this?
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, seekRange, LayerMasks.EnemyLayerMask);

        float minSqrMagnitude = float.MaxValue;
        GameObject closest = null;

        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            if (enemyCollider.gameObject != gameObject 
                && !enemyCollider.gameObject.GetComponent<EnemyHealth>().guarded 
                && enemyCollider.gameObject.GetComponent<KirbyAI>() == null
                && Vector3.SqrMagnitude(enemyCollider.transform.position - transform.position) < minSqrMagnitude)
            {
                minSqrMagnitude = Vector3.SqrMagnitude(enemyCollider.transform.position - transform.position);
                closest = enemyCollider.gameObject;
            }
        }

        return closest == null ? null : closest.GetComponent<EnemyAI>();
    }

    public bool CanGuardEnemy(EnemyAI enemy)
    {
        Vector3 toEnemy = enemy.transform.position - transform.position;
        return enemy != null && (enemy.transform.position - transform.position).sqrMagnitude < squaredGuardRange
            && Physics2D.Raycast(transform.position, toEnemy, toEnemy.magnitude, LayerMasks.WallLayerMask).transform == null;
    }

    public bool CanSeeEnemy(EnemyAI enemy)
    {
        Vector3 toEnemy = enemy.transform.position - transform.position;
        return (enemy.transform.position - transform.position).sqrMagnitude < squaredGuardRange
            && Physics2D.Raycast(transform.position, toEnemy, toEnemy.magnitude, LayerMasks.WallLayerMask).transform == null;
    }

    void OnDisable()
    {
        if (guardedEnemy != null)
        {
            guardedEnemy.GetComponent<EnemyHealth>().guarded = false;
        }
    }

    public void Approach(Vector3 target)
    {
        CancelFreeze();

        // TODO: Move this into the util
        if ((transform.position - target).sqrMagnitude < .4f)
        {
            //Debug.Log("distance from position " + transform.position + " to target " + target + " sqrMagnitude is " + (transform.position - target).sqrMagnitude);
            rb2d.velocity = Vector2.zero;
            return;
        }

        if (EnemyUtil.CanSee(transform.position, target) &&
            EnemyUtil.PathIsNotBlocked(boxCollider2D, transform.position, target))
        {
            //rb2d.velocity = EnemyUtil.CalculateVelocity(transform, target, speed);
            rb2d.velocity = (target - transform.position).normalized * actualSpeed;
        }
        else
        {
            EnemyUtil.ExecuteAStar(transform, target, rb2d, ref lastPathfindTime, pathFindingRate, actualSpeed, true);
        }
    }

    public void Freeze()
    {
        Invoke("DoFreeze", .5f);
    }

    public void CancelFreeze()
    {
        CancelInvoke("DoFreeze");
    }

    private void DoFreeze()
    {
        rb2d.velocity = Vector2.zero;
    }
}
