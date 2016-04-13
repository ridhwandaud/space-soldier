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
        if (!enemy.Firing)
        {
            enemy.Firing = true;
            enemy.StartCoroutine(enemy.CircleAttack());
        }
    }
}
