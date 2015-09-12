using UnityEngine;
using SpriteTile;
using System.Collections.Generic;

public class BasicLevelPopulator : ILevelPopulator
{
    private static int MinimumGridDistanceFromPlayer = 5;

    public void populateLevel(int levelIndex, List<Vector2> openPositions, Vector2 playerSpawn)
    {
        Vector2 playerPosition = AStar.positionToArrayIndicesVector(playerSpawn);

        GameObject enemyPrefab = Resources.Load("Enemy") as GameObject;
        GameObject footSoldierPrefab = Resources.Load("FootSoldier") as GameObject;
        GameObject gordoPrefab = Resources.Load("Gordo") as GameObject;

        List<EnemySpawnData> result = new List<EnemySpawnData>();
        EnemySpawnData basicEnemySpawn = new EnemySpawnData(6, 6, enemyPrefab);
        EnemySpawnData footSoldierSpawn = new EnemySpawnData(7, 9, footSoldierPrefab);
        EnemySpawnData gordoSpawn = new EnemySpawnData(7, 7, gordoPrefab);
        result.Add(basicEnemySpawn);
        result.Add(footSoldierSpawn);
        result.Add(gordoSpawn);

        Transform enemyContainer = GameObject.Find("Enemies").transform;

        spawnEnemies(result, openPositions, enemyContainer, playerPosition);
    }

    void spawnEnemies(List<EnemySpawnData> spawnData, List<Vector2> potentialEnemyPositions, Transform enemyContainer,
        Vector2 playerPosition)
    {
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
