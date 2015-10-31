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

        if (enemy.guardedEnemy == null || !guardedEnemyIsInRange(enemy) || !enemy.CanGuardEnemy(enemy.guardedEnemy))
        {
            if (enemy.guardedEnemy != null)
            {
                enemy.guardedEnemy.GetComponent<EnemyHealth>().guarded = false;
            }

            EnemyAI closestSeekableEnemy = enemy.GetClosestSeekableEnemy();
            if (closestSeekableEnemy == null || !enemy.CanGuardEnemy(closestSeekableEnemy))
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

        // Check if guarded enemy is between player and kirbz. Eventually consolidate the raycasts.
        RaycastHit2D enemyBetweenPlayer = Physics2D.Linecast(enemy.transform.position, Player.PlayerTransform.position, LayerMasks.EnemyLayerMask);
        RaycastHit2D wallBetweenPlayer = Physics2D.Linecast(enemy.transform.position, Player.PlayerTransform.position, LayerMasks.WallLayerMask);
        bool kirbyIsInDanger = enemyBetweenPlayer.transform == null || wallBetweenPlayer.transform == null;
        if (kirbyIsInDanger && enemy.guardedEnemy != null)
        {
            // move to other side of the enemy as far as possible
            Vector3 playerToGuardedEnemy = (enemy.guardedEnemy.transform.position - Player.PlayerTransform.position).normalized;
            RaycastHit2D toHidingSpotBehindEnemy = Physics2D.Raycast(enemy.guardedEnemy.transform.position, playerToGuardedEnemy, 3, LayerMasks.WallLayerMask);
            if (toHidingSpotBehindEnemy.transform != null && toHidingSpotBehindEnemy.distance > enemy.boxCollider2D.bounds.size.x)
            {
                enemy.Approach((Vector3)toHidingSpotBehindEnemy.point - playerToGuardedEnemy * (enemy.boxCollider2D.bounds.extents.x + .1f));
                //Debug.Log("Guarding at reduced distance.");
                //Debug.DrawRay(toHidingSpotBehindEnemy.point, -playerToGuardedEnemy * (enemy.boxCollider2D.bounds.extents.x + .1f), Color.blue, 1f);
            }
            else
            {
                enemy.Approach(enemy.guardedEnemy.transform.position + playerToGuardedEnemy * 3);
                //Debug.Log("Guarding at the normal position.");
                //Debug.DrawLine(enemy.guardedEnemy.transform.position, enemy.guardedEnemy.transform.position + playerToGuardedEnemy * 3,
                //    Color.red, 1f);
            }

        }
        else
        {
            //Debug.Log("Stopping");
            enemy.rb2d.velocity = Vector2.zero;
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
