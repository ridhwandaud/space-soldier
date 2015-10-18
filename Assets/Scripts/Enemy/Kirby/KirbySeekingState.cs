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

    public override void Execute(KirbyAI enemy)
    {
        enemy.lineRenderer.enabled = false;

        if (KirbyAI.GetClosestGuardableEnemy(enemy) != null)
        {
            enemy.fsm.ChangeState(KirbyDefendingState.Instance);
            return;
        }
    }
}
