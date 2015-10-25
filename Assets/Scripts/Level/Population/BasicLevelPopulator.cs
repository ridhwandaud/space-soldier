using UnityEngine;
using System.Collections.Generic;

public class BasicLevelPopulator
{
    private static int MinimumGridDistanceFromPlayer = 5;

    public void spawnEnemies(List<SpawnData> spawnData, List<Vector2> potentialSpawnPositions,
        Vector2 playerSpawn)
    {
        // This is no longer semantically correct, since the populator can now lay traps in addition
        // to enemies. Consider renaming, or creating a separate container for traps.
        Transform enemyContainer = GameObject.Find("Enemies").transform;
        Vector2 playerPosition = AStar.positionToArrayIndicesVector(playerSpawn);

        int totalNumEnemiesPlaced = 0;
        foreach (SpawnData spawnDatum in spawnData)
        {
            int numEntitiesOfTypePlaced = 0;
            int count = Random.Range(spawnDatum.min, spawnDatum.max);
            GameObject enemyPrefab = spawnDatum.prefab;

            while (numEntitiesOfTypePlaced < count && potentialSpawnPositions.Count > 0)
            {
                int index = Random.Range(0, potentialSpawnPositions.Count);
                Vector2 spawnPosition = potentialSpawnPositions[index];
                potentialSpawnPositions.RemoveAt(index);

                if (farEnoughFromPlayer(spawnPosition, playerPosition))
                {
                    GameObject obj = MonoBehaviour.Instantiate(enemyPrefab, new Vector3(
                        spawnPosition.x * LoadLevel.TILE_SIZE, spawnPosition.y * LoadLevel.TILE_SIZE, 0),
                        Quaternion.identity) as GameObject;
                    obj.transform.SetParent(enemyContainer);

                    if (spawnDatum.isEnemy)
                    {
                        totalNumEnemiesPlaced++;
                    }

                    numEntitiesOfTypePlaced++;
                    continue;
                }
            }

            if (numEntitiesOfTypePlaced < count)
            {
                Debug.Log("Could not place all " + count + " entities - ran out of valid positions. Placed "
                    + numEntitiesOfTypePlaced + " entities");
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
