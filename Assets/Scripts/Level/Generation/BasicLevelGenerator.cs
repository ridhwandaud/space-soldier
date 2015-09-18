using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelGenerator : ILevelGenerator
{
    public static int numTiles = 200;
    private BasicLevelAlgorithm algorithm = new BasicLevelAlgorithm();
    private BasicLevelPopulator populator = new BasicLevelPopulator();

    public int[,] GenerateLevel(int levelIndex, out Vector3 playerSpawn)
    {
        List<Vector2> openPositions;
        int[,] level = algorithm.ExecuteAlgorithm(200, out openPositions, out playerSpawn);
        populator.spawnEnemies(getEnemySpawnData(levelIndex), openPositions, playerSpawn);

        return level;
    }

    private List<EnemySpawnData> getEnemySpawnData(int levelIndex)
    {
        GameObject enemyPrefab = Resources.Load("Enemy") as GameObject;
        GameObject footSoldierPrefab = Resources.Load("FootSoldier") as GameObject;
        GameObject gordoPrefab = Resources.Load("Gordo") as GameObject;

        List<EnemySpawnData> result = new List<EnemySpawnData>();
        EnemySpawnData basicEnemySpawn = new EnemySpawnData(6, 6, enemyPrefab);
        EnemySpawnData footSoldierSpawn = new EnemySpawnData(4, 5, footSoldierPrefab);
        EnemySpawnData gordoSpawn = new EnemySpawnData(7, 7, gordoPrefab);
        result.Add(basicEnemySpawn);
        result.Add(footSoldierSpawn);
        result.Add(gordoSpawn);

        return result;
    }
}
