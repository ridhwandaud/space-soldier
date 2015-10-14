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

    private EnemyAI guardedEnemy = null;

    public override void Execute(KirbyAI enemy)
    {
        Debug.Log("Defending");

        // I should extract the sqrMagnitude comparison to a utility.
        if(guardedEnemy == null || (enemy.transform.position - guardedEnemy.transform.position).sqrMagnitude > enemy.squaredRange) {
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
                //Debug.Log("rendering line");
            }
        }

        enemy.lineRenderer.SetPosition(0, enemy.transform.position);
        enemy.lineRenderer.SetPosition(1, guardedEnemy.transform.position);
    }
}
