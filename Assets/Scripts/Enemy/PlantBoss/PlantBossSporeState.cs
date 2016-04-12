using UnityEngine;
using System.Collections;

public class PlantBossSporeState : State<PlantBossAI>
{
    private static PlantBossSporeState instance;
    public static PlantBossSporeState Instance
    {
        get
        {
            instance = instance == null ? new PlantBossSporeState() : instance;
            return instance;
        }
    }

    public override void Execute (PlantBossAI enemy)
    {
        if (!enemy.Firing)
        {
            enemy.Firing = true;
            enemy.StartCoroutine(enemy.FireSporeVolley());
        }
    }
}
