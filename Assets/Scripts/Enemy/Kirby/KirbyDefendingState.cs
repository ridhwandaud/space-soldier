using UnityEngine;

public class KirbyDefendingState : State<KirbyAI> {
    private static float guardChaseThresholdSubtrahend = 2f;
    private static float guardChaseTargetDistanceSubtrahend = 3f;

    private static KirbyDefendingState instance;
    public static KirbyDefendingState Instance
    {
        get
        {
            instance = instance == null ? new KirbyDefendingState() : instance;
            return instance;
        }
    }

    public override void Execute(KirbyAI kirby)
    {
        kirby.lineRenderer.enabled = true;

        if (kirby.guardedEnemy == null || !guardedEnemyIsInRange(kirby) || !kirby.CanGuardEnemy(kirby.guardedEnemy))
        {
            if (kirby.guardedEnemy != null)
            {
                kirby.guardedEnemy.GetComponent<EnemyHealth>().guarded = false;
            }

            EnemyAI closestSeekableEnemy = kirby.GetClosestSeekableEnemy();
            if (closestSeekableEnemy == null || !kirby.CanGuardEnemy(closestSeekableEnemy))
            {
                kirby.guardedEnemy = null;
                kirby.fsm.ChangeState(KirbySeekingState.Instance);
                return;
            }
            else
            {
                kirby.guardedEnemy = closestSeekableEnemy;
                kirby.guardedEnemy.GetComponent<EnemyHealth>().guarded = true;
            }
        }

        // Don't let guarded enemy get out of range if it is the only one around.
        Vector2 enemyToKirby = kirby.transform.position - kirby.guardedEnemy.transform.position;
        if (enemyToKirby.sqrMagnitude > Mathf.Pow(kirby.guardRange - guardChaseThresholdSubtrahend, 2) || !kirby.CanSeeEnemy(kirby.guardedEnemy))
        {
            // Match guarded enemy's speed for smoother motion.
            kirby.actualSpeed = kirby.guardedEnemy.speed;
            Vector2 targetSpot = (Vector2)kirby.guardedEnemy.transform.position + enemyToKirby.normalized * (kirby.guardRange - guardChaseTargetDistanceSubtrahend);
            kirby.Approach(targetSpot);
            //Debug.DrawLine(kirby.transform.position, targetSpot, Color.blue, 1f);
        } else
        {
            kirby.Freeze();
            //kirby.rb2d.velocity = Vector2.zero;
        }

        hideFromPlayer(kirby);

        kirby.lineRenderer.SetPosition(0, kirby.transform.position);
        kirby.lineRenderer.SetPosition(1, kirby.guardedEnemy.transform.position);
    }

    void hideFromPlayer(KirbyAI kirby)
    {
        kirby.actualSpeed = kirby.speed;
        // Check if guarded enemy is between player and kirbz. Eventually consolidate the raycasts.
        RaycastHit2D enemyBetweenPlayer = Physics2D.Linecast(kirby.transform.position, Player.PlayerTransform.position, LayerMasks.EnemyLayerMask);
        RaycastHit2D wallBetweenPlayer = Physics2D.Linecast(kirby.transform.position, Player.PlayerTransform.position, LayerMasks.WallLayerMask);
        bool kirbyIsInDanger = enemyBetweenPlayer.transform == null || wallBetweenPlayer.transform == null;
        if (kirbyIsInDanger && kirby.guardedEnemy != null)
        {
            // move to other side of the enemy as far as possible
            Vector3 playerToGuardedEnemy = (kirby.guardedEnemy.transform.position - Player.PlayerTransform.position).normalized;
            RaycastHit2D toHidingSpotBehindEnemy = Physics2D.Raycast(kirby.guardedEnemy.transform.position, playerToGuardedEnemy, kirby.hideDistance, LayerMasks.WallLayerMask);
            if (toHidingSpotBehindEnemy.transform != null && toHidingSpotBehindEnemy.distance > kirby.boxCollider2D.bounds.size.x)
            {
                kirby.Approach((Vector3)toHidingSpotBehindEnemy.point - playerToGuardedEnemy * (kirby.boxCollider2D.bounds.extents.x + .1f));
                //Debug.Log("Guarding at reduced distance.");
                //Debug.DrawRay(toHidingSpotBehindEnemy.point, -playerToGuardedEnemy * (enemy.boxCollider2D.bounds.extents.x + .1f), Color.blue, 1f);
            }
            else
            {
                kirby.Approach(kirby.guardedEnemy.transform.position + playerToGuardedEnemy * kirby.hideDistance);
                //Debug.Log("Guarding at the normal position.");
                //Debug.DrawLine(enemy.guardedEnemy.transform.position, enemy.guardedEnemy.transform.position + playerToGuardedEnemy * 3,
                //    Color.red, 1f);
            }

        }
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
