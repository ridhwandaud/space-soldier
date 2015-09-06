using UnityEngine;
using System.Collections;

public class MeleeEnemyInRangeState : State<MeleeEnemyAI> {
    private static MeleeEnemyInRangeState instance;
    public static MeleeEnemyInRangeState Instance
    {
        get
        {
            instance = instance == null ? new MeleeEnemyInRangeState() : instance;
            return instance;
        }
    }

    public MeleeEnemyInRangeState() : base() {}

    public override void Execute(MeleeEnemyAI enemy)
    {
        if (Vector3.Distance(player.transform.position, enemy.transform.position) > enemy.attackDistance)
        {
            outOfRangeHandler(enemy);
            return;
        }

        if (!enemy.targetIsAssigned)
        {
            noTokenHandler(enemy);
        }
        else
        {
            hasTokenHandler(enemy);
        }
    }

    private void outOfRangeHandler(MeleeEnemyAI enemy)
    {
        enemy.targetIsAssigned = false;
        enemy.shouldWait = false;

        if (enemy.chasing)
        {
            enemy.fsm.ChangeState(MeleeEnemyChasingState.Instance);
        }
        else
        {
            enemy.fsm.ChangeState(MeleeEnemyIdleState.Instance);
        }
    }

    private void noTokenHandler(MeleeEnemyAI enemy)
    {
        if (enemy.shouldWait)
        {
            enemy.StopMovement();
        }
        else
        {
            enemy.Charge(player.transform.position, .8f, .8f, .5f);
        }   
    }

    private void hasTokenHandler(MeleeEnemyAI enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.target) <= .2)
        {
            enemy.StopMovement(); // Instead, go to attacking state.
        }
        else
        {
            enemy.Charge(enemy.target, .8f, .8f, 1f);
        }
    }
}
