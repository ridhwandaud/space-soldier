using UnityEngine;

public class KirbyAI : EnemyAI {
    public float guardRange;
    public float seekRange;
    public FiniteStateMachine<KirbyAI> fsm;
    public LineRenderer lineRenderer;
    public EnemyAI guardedEnemy;

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
	}
	
	void Update () {
        if (!LoadLevel.WALL_COLLIDERS_INITIALIZED)
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
        return enemy != null && (enemy.transform.position - transform.position).sqrMagnitude < squaredGuardRange;
    }

    void OnDisable()
    {
        if (guardedEnemy != null)
        {
            guardedEnemy.GetComponent<EnemyHealth>().guarded = false;
        }
    }
}
