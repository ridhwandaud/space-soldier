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
        stopAttackIfNecessary(enemy);

        if (enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("gordo_attack"))
        {
            enemy.StopMovement();
        }
        else if (Vector3.Distance(Player.PlayerTransform.position, enemy.transform.position) > enemy.attackDistance)
        {
            outOfRangeHandler(enemy);
        }
        else if (!enemy.targetIsAssigned)
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
            enemy.Charge(Player.PlayerTransform.position, .8f, .8f, .5f);
        }   
    }

    private void hasTokenHandler(MeleeEnemyAI enemy)
    {
        if (Vector3.Distance(enemy.transform.position, enemy.target) <= .2)
        {
            enemy.animator.SetBool("Attacking", true);
            enemy.StopMovement();
        }
        else
        {
            enemy.Charge(enemy.target, .8f, .8f, 1f);
        }
    }

    private void stopAttackIfNecessary(MeleeEnemyAI enemy)
    {
        if (Vector3.Distance(Player.PlayerTransform.position, enemy.transform.position) > enemy.attackDistance || !enemy.targetIsAssigned)
        {
            enemy.animator.SetBool("Attacking", false);
        }
    }
}
