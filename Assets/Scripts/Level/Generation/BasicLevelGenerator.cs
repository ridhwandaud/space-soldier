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
    enum BasicLevelDifficulty { Easy, Hard};

    private static int HardLevelThreshold = 4;

    public BasicLevelGenerator()
    {
        basicEnemyPrefab = Resources.Load("Enemy") as GameObject;
        footSoldierPrefab = Resources.Load("FootSoldier") as GameObject;
        gordoPrefab = Resources.Load("Gordo") as GameObject;

        configsBySize[BasicLevelSize.Small] = smallLevelConfig;
        configsBySize[BasicLevelSize.Medium] = mediumLevelConfig;
        configsBySize[BasicLevelSize.Large] = LargeLevelConfig;

        smallLevelConfig[BasicLevelDifficulty.Easy] = smallEasyLevelConfigs;
        mediumLevelConfig[BasicLevelDifficulty.Easy] = mediumEasyLevelConfigs;
        LargeLevelConfig[BasicLevelDifficulty.Easy] = LargeEasyLevelConfigs;
    }

    public int[,] GenerateLevel(int levelIndex, out Vector3 playerSpawn)
    {
        List<Vector2> openPositions;
        BasicLevelSize size = getLevelSize();
        int[,] level = algorithm.ExecuteAlgorithm((int)size, out openPositions, out playerSpawn);
        populator.spawnEnemies(getEnemySpawnData(size, levelIndex), openPositions, playerSpawn);

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

    private List<EnemySpawnData> getEnemySpawnData(BasicLevelSize size, int levelIndex)
    {
        BasicLevelDifficulty difficulty = levelIndex < HardLevelThreshold ? BasicLevelDifficulty.Easy : BasicLevelDifficulty.Hard;

        List<EnemySpawnConfig> possibleConfigs = configsBySize[size][difficulty];
        EnemySpawnConfig config = possibleConfigs[Random.Range(0, possibleConfigs.Count - 1)];

        return new List<EnemySpawnData> { 
            new EnemySpawnData(config.basicEnemyMinMax.x, config.basicEnemyMinMax.y, basicEnemyPrefab),
            new EnemySpawnData(config.footSoldierMinMax.x, config.footSoldierMinMax.y, footSoldierPrefab),
            new EnemySpawnData(config.gordoMinMax.x, config.gordoMinMax.y, gordoPrefab)
        };
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

    private Dictionary<BasicLevelSize, Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>> configsBySize = 
        new Dictionary<BasicLevelSize,Dictionary<BasicLevelDifficulty,List<EnemySpawnConfig>>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> smallLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> mediumLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> LargeLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private List<EnemySpawnConfig> smallEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(3, 4), new Int2(3, 4), new Int2(3, 4)),
        new EnemySpawnConfig(new Int2(2, 4), new Int2(0, 0), new Int2(5, 6)),
        new EnemySpawnConfig(new Int2(0, 1), new Int2(2, 2), new Int2(2, 3)),
    };
    private List<EnemySpawnConfig> mediumEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(3, 5), new Int2(3, 5), new Int2(3, 5))
    };
    private List<EnemySpawnConfig> LargeEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(5, 7), new Int2(5, 7), new Int2(5, 7))
    };
}
