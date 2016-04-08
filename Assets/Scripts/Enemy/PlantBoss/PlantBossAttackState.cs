using UnityEngine;
using System.Collections;

public class PlantBossAttackState : State<PlantBossAI>
{
    private static PlantBossAttackState instance;
    public static PlantBossAttackState Instance
    {
        get
        {
            instance = instance == null ? new PlantBossAttackState() : instance;
            return instance;
        }
    }

    public override void Execute (PlantBossAI enemy)
    {
        if (!enemy.firing)
        {
            enemy.firing = true;
            enemy.StartCoroutine(enemy.FireVolley());
        }
    }
}
