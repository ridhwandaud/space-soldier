using UnityEngine;

public class KirbyAI : EnemyAI {
    public float range;
    public float squaredRange;
    public FiniteStateMachine<KirbyAI> fsm;
    public LineRenderer lineRenderer;
    public EnemyAI guardedEnemy;

    void Awake () {
        squaredRange = range * range;
        lineRenderer = GetComponent<LineRenderer>();
        fsm = new FiniteStateMachine<KirbyAI>(this, KirbySeekingState.Instance);
	}
	
	void Update () {
        if (!LoadLevel.WALL_COLLIDERS_INITIALIZED)
        {
            return;
        }

        fsm.Update();
	}

    public static EnemyAI GetClosestGuardableEnemy(KirbyAI enemy)
    {
        // Should this happen at an interval, rather than every update loop? How expensive is this?
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.range, LayerMasks.EnemyLayerMask);

        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            if (enemyCollider.gameObject != enemy.gameObject && !enemyCollider.gameObject.GetComponent<EnemyHealth>().guarded)
            {
                return enemyCollider.gameObject.GetComponent<EnemyAI>();
            }
        }

        return null;
    }

    void OnDisable()
    {
        if (guardedEnemy != null)
        {
            guardedEnemy.GetComponent<EnemyHealth>().guarded = false;
        }
    }
}
