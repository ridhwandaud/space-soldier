using UnityEngine;
using System.Collections;

public class PlantBossSleepState : State<PlantBossAI> {
    private static PlantBossSleepState instance;
    public static PlantBossSleepState Instance
    {
        get
        {
            instance = instance == null ? new PlantBossSleepState() : instance;
            return instance;
        }
    }
    public override void Execute (PlantBossAI enemy)
    {
        if (enemy.Awakened)
        {
            enemy.Fsm.ChangeState(PlantBossSeedState.Instance);
        }
    }
}
