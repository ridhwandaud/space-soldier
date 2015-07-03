using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {

    public static void spawnEnemies(List<EnemySpawnData> spawnData, List<Vector2> potentialEnemyPositions)
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

                Instantiate(enemyPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0),
                    Quaternion.identity);

                numEnemiesPlaced++;
            }
        }
    }
}
