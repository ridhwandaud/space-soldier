using UnityEngine;

public class PlantBossSeedState : State<PlantBossAI> {

    private static PlantBossSeedState instance;
    public static PlantBossSeedState Instance
    {
        get
        {
            instance = instance == null ? new PlantBossSeedState() : instance;
            return instance;
        }
    }

    public override void Execute (PlantBossAI enemy)
    {
        float angle = Random.Range(0, 360);
        for (int i = 0; i < enemy.Seeds.Count; i++)
        {
            angle += Random.Range(40, 100);
            enemy.Seeds[i].Launch(enemy.transform.position, VectorUtil.RotateVector(Vector2.right, angle * Mathf.Deg2Rad),
                enemy.initialSeedSpeed);
            GameState.NumEnemiesRemaining++;
        }

        enemy.Fsm.ChangeState(PlantBossSporeState.Instance);
    }
}
