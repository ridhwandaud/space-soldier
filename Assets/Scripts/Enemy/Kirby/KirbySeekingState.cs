using UnityEngine;
using System.Collections;

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

    public override void Execute(KirbyAI enemy)
    {
        enemy.lineRenderer.enabled = false;

        // Should this happen at an interval, rather than every update loop? How expensive is this?
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.range, LayerMasks.EnemyLayerMask);

        if (nearbyEnemies.Length != 0 && !(nearbyEnemies.Length == 1 && nearbyEnemies[0].transform == enemy.transform))
        {
            enemy.fsm.ChangeState(KirbyDefendingState.Instance);
            return;
        }

        //if ((enemy.transform.position - Player.PlayerTransform.position).sqrMagnitude <= enemy.squaredRange)
        //{
        //    Debug.Log("Attacking");
        //    enemy.fsm.ChangeState(KirbyAttackingState.Instance);
        //}
    }
}
