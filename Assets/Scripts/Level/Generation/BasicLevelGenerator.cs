using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelGenerator : ILevelGenerator
{
    private BasicLevelAlgorithm algorithm = new BasicLevelAlgorithm();
    private BasicLevelPopulator populator = new BasicLevelPopulator();

    GameObject basicEnemyPrefab;
    GameObject footSoldierPrefab;
    GameObject gordoPrefab;

    enum BasicLevelSize { Small = 80, Medium = 100, Large = 200 };
    enum BasicLevelDifficulty { Easy, Medium, Hard };

    public BasicLevelGenerator()
    {
        basicEnemyPrefab = Resources.Load("Enemy") as GameObject;
        footSoldierPrefab = Resources.Load("FootSoldier") as GameObject;
        gordoPrefab = Resources.Load("Gordo") as GameObject;
    }

    public int[,] GenerateLevel(int levelIndex, out Vector3 playerSpawn)
    {
        List<Vector2> openPositions;
        BasicLevelSize size = getLevelSize();
        int[,] level = algorithm.ExecuteAlgorithm((int)size, out openPositions, out playerSpawn);
        populator.spawnEnemies(getEnemySpawnData(levelIndex), openPositions, playerSpawn);

        return level;
    }

    private BasicLevelSize getLevelSize()
    {
        int rand = Random.Range(1, 3);

        if (rand == 1)
        {
            return BasicLevelSize.Small;
        }
        else if (rand == 2)
        {
            return BasicLevelSize.Medium;
        }
        else
        {
            return BasicLevelSize.Large;
        }
    }

    private List<EnemySpawnData> getEnemySpawnData(int levelIndex)
    {
        List<EnemySpawnData> result = new List<EnemySpawnData>();
        EnemySpawnData basicEnemySpawn = new EnemySpawnData(6, 6, basicEnemyPrefab);
        EnemySpawnData footSoldierSpawn = new EnemySpawnData(4, 5, footSoldierPrefab);
        EnemySpawnData gordoSpawn = new EnemySpawnData(7, 7, gordoPrefab);
        result.Add(basicEnemySpawn);
        result.Add(footSoldierSpawn);
        result.Add(gordoSpawn);


        return result;
    }

    private struct EnemySpawnConfig
    {
        public Int2 basicEnemyMinMax;
        public Int2 footSoldierMinMax;
        public Int2 gordoMinMax;

        public EnemySpawnConfig(Int2 basicEnemyMinMax, Int2 footSoldierMinMax, Int2 gordoMinMax)
        {
            this.basicEnemyMinMax = basicEnemyMinMax;
            this.footSoldierMinMax = footSoldierMinMax;
            this.gordoMinMax = gordoMinMax;
        }
    }

    private Dictionary<BasicLevelSize, Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>> enemyConfigs;
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> smallLevelConfig;
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> mediumLevelConfig;
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> LargeLevelConfig;
    private List<EnemySpawnConfig> smallEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(4, 5), new Int2(0, 0), new Int2(3, 4))
    };
    private List<EnemySpawnConfig> mediumEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        //new EnemySpawnConfig(new Int2())
    };
}
