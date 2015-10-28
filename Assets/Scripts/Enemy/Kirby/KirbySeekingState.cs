using UnityEngine;

public class KirbySeekingState : State<KirbyAI> {
    private static KirbySeekingState instance;
    public static KirbySeekingState Instance
    {
        get
        {
            instance = instance == null ? new KirbySeekingState() : instance;
            return instance;
        }
    }

    public override void Execute(KirbyAI kirby)
    {
        kirby.lineRenderer.enabled = false;

        EnemyAI closestSeekableEnemy = kirby.GetClosestSeekableEnemy();
        if (kirby.CanGuardEnemy(closestSeekableEnemy))
        {
            kirby.rb2d.velocity = Vector2.zero;
            kirby.fsm.ChangeState(KirbyDefendingState.Instance);
            return;
        } else if (closestSeekableEnemy != null)
        {
            Approach(kirby, closestSeekableEnemy.transform.position);
        } else
        {
            kirby.rb2d.velocity = Vector2.zero;
        }
    }

    public void Approach(KirbyAI kirby, Vector2 target)
    {
        if (EnemyUtil.CanSee(kirby.transform.position, Player.PlayerTransform.position) &&
            EnemyUtil.PathIsNotBlocked(kirby.boxCollider2D, kirby.transform.position, Player.PlayerTransform.position))
        {
            kirby.rb2d.velocity = EnemyUtil.CalculateVelocity(kirby.transform, target, kirby.speed);
        }
        else
        {
            EnemyUtil.ExecuteAStar(kirby.transform, target, kirby.rb2d, ref kirby.lastPathfindTime, kirby.pathFindingRate, kirby.speed);
        }
    }
}
