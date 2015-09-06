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
        float distanceFromPlayer = Vector3.Distance(player.transform.position, enemy.transform.position);

        if (EnemyUtil.CanSee(enemy.transform.position, player.transform.position) && distanceFromPlayer <= enemy.attackDistance)
        {
            enemy.fsm.ChangeState(MeleeEnemyInRangeState.Instance);
        }
        else if (EnemyUtil.CanSee(enemy.transform.position, player.transform.position) && distanceFromPlayer <= enemy.chargeDistance)
        {
            enemy.fsm.ChangeState(MeleeEnemyChasingState.Instance);
        }
        else
        {
            enemy.CalculateVelocity(enemy.transform.position);
        }
    }
}
