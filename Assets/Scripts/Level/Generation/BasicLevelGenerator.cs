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
    GameObject sniperPrefab;
    GameObject gordoTrapPrefab;

    enum BasicLevelSize { Small = 80, Medium = 100, Large = 200 };
    enum BasicLevelDifficulty { Easy, Hard};

    private static int HardLevelThreshold = 4;

    public BasicLevelGenerator()
    {
        basicEnemyPrefab = Resources.Load("Enemy") as GameObject;
        footSoldierPrefab = Resources.Load("FootSoldier") as GameObject;
        gordoPrefab = Resources.Load("Gordo") as GameObject;
        sniperPrefab = Resources.Load("Sniper") as GameObject;
        gordoTrapPrefab = Resources.Load("GordoTrap") as GameObject;

        configsBySize[BasicLevelSize.Small] = smallLevelConfig;
        configsBySize[BasicLevelSize.Medium] = mediumLevelConfig;
        configsBySize[BasicLevelSize.Large] = LargeLevelConfig;

        smallLevelConfig[BasicLevelDifficulty.Easy] = smallEasyLevelConfigs;
        mediumLevelConfig[BasicLevelDifficulty.Easy] = mediumEasyLevelConfigs;
        LargeLevelConfig[BasicLevelDifficulty.Easy] = largeEasyLevelConfigs;
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
        int rand = Random.Range(1, 1);

        if (rand == 1)
        {
            Debug.Log("level size: small");
            return BasicLevelSize.Small;
        }
        else if (rand == 2)
        {
            Debug.Log("level size: medium");
            return BasicLevelSize.Medium;
        }
        else
        {
            Debug.Log("level size: large");
            return BasicLevelSize.Large;
        }
    }

    private List<SpawnData> getEnemySpawnData(BasicLevelSize size, int levelIndex)
    {
        BasicLevelDifficulty difficulty = levelIndex < HardLevelThreshold ? BasicLevelDifficulty.Easy : BasicLevelDifficulty.Hard;

        List<EnemySpawnConfig> possibleConfigs = configsBySize[size][difficulty];
        EnemySpawnConfig config = possibleConfigs[Random.Range(0, possibleConfigs.Count - 1)];

        config.printConfig();

        return new List<SpawnData> {
            new SpawnData(1, 1, sniperPrefab)
        };

        return new List<SpawnData> { 
            new SpawnData(config.basicEnemyMinMax.x, config.basicEnemyMinMax.y, basicEnemyPrefab),
            new SpawnData(config.footSoldierMinMax.x, config.footSoldierMinMax.y, footSoldierPrefab),
            new SpawnData(config.gordoMinMax.x, config.gordoMinMax.y, gordoPrefab),
            new SpawnData(config.gordoTrapMinMax.x, config.gordoTrapMinMax.y, gordoTrapPrefab, false)
        };
    }

    private struct EnemySpawnConfig
    {
        public Int2 basicEnemyMinMax;
        public Int2 footSoldierMinMax;
        public Int2 gordoMinMax;
        public Int2 gordoTrapMinMax;

        public EnemySpawnConfig(Int2 basicEnemyMinMax, Int2 footSoldierMinMax, Int2 gordoMinMax,
            Int2 gordoTrapMinMax)
        {
            this.basicEnemyMinMax = basicEnemyMinMax;
            this.footSoldierMinMax = footSoldierMinMax;
            this.gordoMinMax = gordoMinMax;
            this.gordoTrapMinMax = gordoTrapMinMax;
        }

        public void printConfig()
        {
            Debug.Log("basic enemy min and max: " + basicEnemyMinMax.x + ", " + basicEnemyMinMax.y);
            Debug.Log("foot soldier min and max: " + footSoldierMinMax.x + ", " + footSoldierMinMax.y);
            Debug.Log("gordo min and max: " + gordoMinMax.x + ", " + gordoMinMax.y);
            Debug.Log("gordoTrap min and max: " + gordoTrapMinMax.x + ", " + gordoTrapMinMax.y);
        }
    }

    private Dictionary<BasicLevelSize, Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>> configsBySize = 
        new Dictionary<BasicLevelSize,Dictionary<BasicLevelDifficulty,List<EnemySpawnConfig>>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> smallLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> mediumLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> LargeLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private List<EnemySpawnConfig> smallEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(3, 4), new Int2(3, 4), new Int2(3, 4), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(2, 4), new Int2(0, 0), new Int2(5, 6), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(0, 1), new Int2(2, 2), new Int2(3, 4), new Int2(1, 2)),
    };
    private List<EnemySpawnConfig> mediumEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(5, 6), new Int2(5, 6), new Int2(5, 6), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(6, 7), new Int2(3, 5), new Int2(8, 9), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(3, 5), new Int2(3, 5), new Int2(3, 5), new Int2(1, 2))
    };
    private List<EnemySpawnConfig> largeEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(6, 7), new Int2(4, 5), new Int2(6, 7), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(5, 7), new Int2(3, 4), new Int2(9, 9), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(3, 4), new Int2(8, 9), new Int2(3, 4), new Int2(1, 2))
    };
}
