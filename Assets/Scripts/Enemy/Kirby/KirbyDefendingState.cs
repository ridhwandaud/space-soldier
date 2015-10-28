using UnityEngine;

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

    public override void Execute(KirbyAI enemy)
    {
        enemy.lineRenderer.enabled = true;

        if (enemy.guardedEnemy == null || !guardedEnemyIsInRange(enemy))
        {
            if (enemy.guardedEnemy != null)
            {
                //Debug.Break();
                enemy.guardedEnemy.GetComponent<EnemyHealth>().guarded = false;
            }

            EnemyAI closestSeekableEnemy = enemy.GetClosestSeekableEnemy();
            if (!enemy.CanGuardEnemy(closestSeekableEnemy))
            {
                enemy.guardedEnemy = null;
                enemy.fsm.ChangeState(KirbySeekingState.Instance);
                return;
            }
            else
            {
                enemy.guardedEnemy = closestSeekableEnemy;
                enemy.guardedEnemy.GetComponent<EnemyHealth>().guarded = true;
            }
        }

        enemy.lineRenderer.SetPosition(0, enemy.transform.position);
        enemy.lineRenderer.SetPosition(1, enemy.guardedEnemy.transform.position);
    }

    bool guardedEnemyIsInRange(KirbyAI enemy)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(enemy.transform.position, enemy.guardedEnemy.transform.position - enemy.transform.position, enemy.guardRange, LayerMasks.EnemyLayerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.transform.gameObject == enemy.guardedEnemy.gameObject)
            {
                return true;
            }
        }

        return false;
    }
}
