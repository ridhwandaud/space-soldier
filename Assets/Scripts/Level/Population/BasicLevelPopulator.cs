using UnityEngine;
using System.Collections.Generic;

public class BasicLevelPopulator : ILevelPopulator
{
    public void populateLevel(int levelIndex, List<Vector2> openPositions)
    {
        GameObject enemyPrefab = Resources.Load("Enemy") as GameObject;
        List<EnemySpawnData> result = new List<EnemySpawnData>();
        EnemySpawnData basicEnemySpawn = new EnemySpawnData(12, 15, enemyPrefab);
        result.Add(basicEnemySpawn);

        Transform enemyContainer = GameObject.Find("Enemies").transform;

        spawnEnemies(result, openPositions, enemyContainer);
    }

    void spawnEnemies(List<EnemySpawnData> spawnData, List<Vector2> potentialEnemyPositions, Transform enemyContainer)
    {
        foreach (EnemySpawnData spawnDatum in spawnData)
        {
            int numEnemiesPlaced = 0;
            int count = Random.Range(spawnDatum.min, spawnDatum.max);
            GameObject enemyPrefab = spawnDatum.enemyType;

            while (numEnemiesPlaced < count)
            {
                int index = Random.Range(0, potentialEnemyPositions.Count);
                Vector2 spawnPosition = potentialEnemyPositions[index];
                potentialEnemyPositions.RemoveAt(index);

                GameObject obj = MonoBehaviour.Instantiate(enemyPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0),
                    Quaternion.identity) as GameObject;
                obj.transform.SetParent(enemyContainer);

                numEnemiesPlaced++;
            }
        }
    }
}
