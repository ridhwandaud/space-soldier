using UnityEngine;
using SpriteTile;
using System.Collections.Generic;

public class BasicLevelPopulator
{
    private static int MinimumGridDistanceFromPlayer = 5;

    public void spawnEnemies(List<EnemySpawnData> spawnData, List<Vector2> potentialEnemyPositions,
        Vector2 playerSpawn)
    {
        Transform enemyContainer = GameObject.Find("Enemies").transform;
        Vector2 playerPosition = AStar.positionToArrayIndicesVector(playerSpawn);

        int totalNumEnemiesPlaced = 0;
        foreach (EnemySpawnData spawnDatum in spawnData)
        {
            int numEnemiesOfTypePlaced = 0;
            int count = Random.Range(spawnDatum.min, spawnDatum.max);
            totalNumEnemiesPlaced += count;
            GameObject enemyPrefab = spawnDatum.enemyType;

            while (numEnemiesOfTypePlaced < count && potentialEnemyPositions.Count > 0)
            {
                int index = Random.Range(0, potentialEnemyPositions.Count);
                Vector2 spawnPosition = potentialEnemyPositions[index];
                potentialEnemyPositions.RemoveAt(index);

                if (farEnoughFromPlayer(spawnPosition, playerPosition))
                {
                    GameObject obj = MonoBehaviour.Instantiate(enemyPrefab, new Vector3(
                        spawnPosition.x * LoadLevel.TILE_SIZE, spawnPosition.y * LoadLevel.TILE_SIZE, 0),
                        Quaternion.identity) as GameObject;
                    obj.transform.SetParent(enemyContainer);

                    numEnemiesOfTypePlaced++;
                    continue;
                }
            }

            if (numEnemiesOfTypePlaced < count)
            {
                Debug.Log("Could not place all " + count + " enemies - ran out of valid positions. Placed "
                    + numEnemiesOfTypePlaced + " enemies");
            }
        }

        GameState.NumEnemiesRemaining = totalNumEnemiesPlaced;
    }

    bool farEnoughFromPlayer(Vector2 enemyPosition, Vector2 playerPosition)
    {
        return (Mathf.Abs(playerPosition.x - enemyPosition.x)
            + Mathf.Abs(playerPosition.y - enemyPosition.y)) > MinimumGridDistanceFromPlayer;
    }
}
