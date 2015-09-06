using UnityEngine;
using System.Collections;

public class MeleeEnemyChasingState : State<MeleeEnemyAI> {
    private static MeleeEnemyChasingState instance;
    public static MeleeEnemyChasingState Instance
    {
        get 
        {
            instance = instance == null ? new MeleeEnemyChasingState() : instance;
            return instance;
        }
    }

    public MeleeEnemyChasingState() : base() {}

    public override void Execute(MeleeEnemyAI enemy)
    {
        float distanceFromPlayer = Vector3.Distance(player.transform.position, enemy.transform.position);

        if (distanceFromPlayer < enemy.attackDistance)
        {
            enemy.fsm.ChangeState(MeleeEnemyInRangeState.Instance);
            return;
        }

        if (EnemyUtil.CanSee(enemy.transform.position, player.transform.position) && distanceFromPlayer <= enemy.chargeDistance)
        {
            enemy.chasing = true;
            enemy.CancelInvoke("DeactivateChase");

            enemy.Charge(player.transform.position, 1, 1, 1);
        }

        else
        {
            enemy.Invoke("DeactivateChase", enemy.chaseTime);
            if (distanceFromPlayer <= enemy.chargeDistance && enemy.chasing)
            {
                enemy.ExecuteAStar(player.transform.position);
            }
            else
            {
                enemy.fsm.ChangeState(MeleeEnemyIdleState.Instance);
            }
        }
    }
}
