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

    public override void Execute(MeleeEnemyAI enemy)
    {
        float distanceFromPlayer = Vector3.Distance(Player.PlayerTransform.position, enemy.transform.position);

        if (distanceFromPlayer < enemy.attackDistance)
        {
            enemy.fsm.ChangeState(MeleeEnemyInRangeState.Instance);
            return;
        }

        if (EnemyUtil.CanSee(enemy.transform.position, Player.PlayerTransform.position) && distanceFromPlayer <= enemy.chargeDistance)
        {
            enemy.chasing = true;
            enemy.CancelInvoke("DeactivateChase");

            enemy.Charge(Player.PlayerTransform.position, 1, 1, 1);
        }
        else
        {
            enemy.Invoke("DeactivateChase", enemy.chaseTime);
            if (enemy.chasing) {
                if (EnemyUtil.CanSee(enemy.transform.position, Player.PlayerTransform.position))
                {
                    enemy.Charge(Player.PlayerTransform.position, 1, 1, 1);
                }
                else
                {
                    enemy.ExecuteAStar(Player.PlayerTransform.position);
                }
            }
            else
            {
                enemy.fsm.ChangeState(MeleeEnemyIdleState.Instance);
            }
        }
    }
}
