using UnityEngine;
using System.Collections;

public class MeleeEnemyIdleState : State<MeleeEnemyAI> {
    private static MeleeEnemyIdleState instance;
    public static MeleeEnemyIdleState Instance
    {
        get
        {
            instance = instance == null ? new MeleeEnemyIdleState() : instance;
            return instance;
        }
    }


    public override void Execute(MeleeEnemyAI enemy)
    {
        float distanceFromPlayer = Vector3.Distance(Player.PlayerTransform.position, enemy.transform.position);

        if (EnemyUtil.CanSee(enemy.transform.position, Player.PlayerTransform.position) && distanceFromPlayer <= enemy.attackDistance)
        {
            enemy.fsm.ChangeState(MeleeEnemyInRangeState.Instance);
        }
        else if (enemy.chasing || EnemyUtil.CanSee(enemy.transform.position, Player.PlayerTransform.position)
            && distanceFromPlayer <= enemy.chargeDistance)
        {
            enemy.fsm.ChangeState(MeleeEnemyChasingState.Instance);
        }
        else
        {
            enemy.CalculateVelocity(enemy.transform.position);
        }
    }
}
