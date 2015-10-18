using UnityEngine;
using System.Collections;

public class KirbyDefendingState : State<KirbyAI> {
    private static KirbyDefendingState instance;
    public static KirbyDefendingState Instance
    {
        get
        {
            instance = instance == null ? new KirbyDefendingState() : instance;
            return instance;
        }
    }

    // This needs to be moved into the KirbyAI, since this state is a singleton.
    private EnemyAI guardedEnemy = null;

    public override void Execute(KirbyAI enemy)
    {
        enemy.lineRenderer.enabled = true;

        // I should extract the sqrMagnitude comparison to a utility.
        if (guardedEnemy == null || guardedEnemyIsInRange(enemy))
        {
            Debug.Log("Re-evaluating");
            if (guardedEnemy != null)
            {
                guardedEnemy.GetComponent<EnemyHealth>().invincible = false;
            }
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.range, LayerMasks.EnemyLayerMask);
            if (nearbyEnemies.Length == 0)
            {
                if ((enemy.transform.position - Player.PlayerTransform.position).sqrMagnitude <= enemy.squaredRange)
                {
                    //enemy.fsm.ChangeState(KirbyAttackingState.Instance);
                    enemy.lineRenderer.SetPosition(0, enemy.transform.position);
                    enemy.lineRenderer.SetPosition(1, Player.PlayerTransform.position);
                }
                else
                {
                    enemy.fsm.ChangeState(KirbySeekingState.Instance);
                }
            }
            else
            {
                guardedEnemy = nearbyEnemies[0].gameObject.GetComponent<EnemyAI>();
                guardedEnemy.GetComponent<EnemyHealth>().invincible = true;
            }
        }

        enemy.lineRenderer.SetPosition(0, enemy.transform.position);
        enemy.lineRenderer.SetPosition(1, guardedEnemy.transform.position);
    }

    bool guardedEnemyIsInRange(KirbyAI enemy)
    {
        Vector3 point = enemy.transform.position + (guardedEnemy.transform.position - enemy.transform.position) * enemy.range;

        // I should probably cache this.
        return guardedEnemy.GetComponent<BoxCollider2D>().bounds.Contains(point);
    }
}
