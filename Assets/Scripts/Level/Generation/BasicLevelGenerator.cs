using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelGenerator : ILevelGenerator
{
    private BasicLevelAlgorithm algorithm = new BasicLevelAlgorithm();
    private BasicLevelPopulator populator = new BasicLevelPopulator();
    private BasicLevelDecorator decorator = new BasicLevelDecorator();

    GameObject basicEnemyPrefab;
    GameObject footSoldierPrefab;
    GameObject gordoPrefab;
    GameObject sniperPrefab;
    GameObject gordoTrapPrefab;
    GameObject knightPrefab;
    GameObject kirbyPrefab;
    GameObject troopaPrefab;

    enum BasicLevelSize { Small = 200, Medium = 350, Large = 500 };
    enum BasicLevelDifficulty { Easy, Hard};

    private static int HardLevelThreshold = 4;

    public BasicLevelGenerator()
    {
        basicEnemyPrefab = Resources.Load("Enemy") as GameObject;
        footSoldierPrefab = Resources.Load("FootSoldier") as GameObject;
        gordoPrefab = Resources.Load("Gordo") as GameObject;
        sniperPrefab = Resources.Load("Sniper") as GameObject;
        gordoTrapPrefab = Resources.Load("GordoTrap") as GameObject;
        knightPrefab = Resources.Load("Knight") as GameObject;
        kirbyPrefab = Resources.Load("Kirby") as GameObject;
        troopaPrefab = Resources.Load("Troopa") as GameObject;

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
        decorator.CreateTilemap(level, barrierTileDictionary);

        return level;
    }

    private BasicLevelSize getLevelSize()
    {
        int rand = Random.Range(0, 2);
        rand = 1;

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

        //return new List<SpawnData> {
        //    new SpawnData(3, 3, troopaPrefab)
        //};

        return new List<SpawnData> {
            new SpawnData(config.basicEnemyMinMax.x, config.basicEnemyMinMax.y, basicEnemyPrefab),
            new SpawnData(config.footSoldierMinMax.x, config.footSoldierMinMax.y, footSoldierPrefab),
            new SpawnData(config.gordoMinMax.x, config.gordoMinMax.y, gordoPrefab),
            new SpawnData(config.gordoTrapMinMax.x, config.gordoTrapMinMax.y, gordoTrapPrefab, false),
            new SpawnData(config.kirbyMinMax.x, config.kirbyMinMax.y, kirbyPrefab),
            new SpawnData(config.sniperMinMax.x, config.sniperMinMax.y, sniperPrefab)
        };
    }

    private struct EnemySpawnConfig
    {
        public Int2 basicEnemyMinMax;
        public Int2 footSoldierMinMax;
        public Int2 gordoMinMax;
        public Int2 gordoTrapMinMax;
        public Int2 sniperMinMax;
        public Int2 kirbyMinMax;

        public EnemySpawnConfig(Int2 basicEnemyMinMax, Int2 footSoldierMinMax, Int2 gordoMinMax,
            Int2 gordoTrapMinMax, Int2 sniperMinMax, Int2 kirbyMinMax)
        {
            this.basicEnemyMinMax = basicEnemyMinMax;
            this.footSoldierMinMax = footSoldierMinMax;
            this.gordoMinMax = gordoMinMax;
            this.gordoTrapMinMax = gordoTrapMinMax;
            this.sniperMinMax = sniperMinMax;
            this.kirbyMinMax = kirbyMinMax;
        }

        public void printConfig()
        {
            Debug.Log("basic enemy min and max: " + basicEnemyMinMax.x + ", " + basicEnemyMinMax.y);
            Debug.Log("foot soldier min and max: " + footSoldierMinMax.x + ", " + footSoldierMinMax.y);
            Debug.Log("gordo min and max: " + gordoMinMax.x + ", " + gordoMinMax.y);
            Debug.Log("gordoTrap min and max: " + gordoTrapMinMax.x + ", " + gordoTrapMinMax.y);
            Debug.Log("kirby min and max: " + kirbyMinMax.x + ", " + kirbyMinMax.y);
            Debug.Log("sniper min and max: " + sniperMinMax.x + ", " + sniperMinMax.y);
        }
    }

    private Dictionary<BasicLevelSize, Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>> configsBySize = 
        new Dictionary<BasicLevelSize,Dictionary<BasicLevelDifficulty,List<EnemySpawnConfig>>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> smallLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> mediumLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>> LargeLevelConfig = new Dictionary<BasicLevelDifficulty, List<EnemySpawnConfig>>();
    private List<EnemySpawnConfig> smallEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(2, 3), new Int2(1, 3), new Int2(2, 3), new Int2(1, 1), new Int2(0, 0), new Int2(0, 1)),
        new EnemySpawnConfig(new Int2(2, 4), new Int2(0, 0), new Int2(3, 4), new Int2(1, 2), new Int2(0, 0), new Int2(0, 1)),
        new EnemySpawnConfig(new Int2(0, 1), new Int2(2, 2), new Int2(3, 4), new Int2(1, 2), new Int2(0, 0), new Int2(0, 1)),
    };
    private List<EnemySpawnConfig> mediumEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(5, 6), new Int2(5, 6), new Int2(5, 6), new Int2(1, 2), new Int2(1, 1), new Int2(1, 1)),
        new EnemySpawnConfig(new Int2(6, 7), new Int2(3, 5), new Int2(8, 9), new Int2(1, 2), new Int2(1, 1), new Int2(1, 1)),
        new EnemySpawnConfig(new Int2(3, 5), new Int2(3, 5), new Int2(3, 5), new Int2(1, 2), new Int2(1, 1), new Int2(1, 1))
    };
    private List<EnemySpawnConfig> largeEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(6, 7), new Int2(4, 5), new Int2(6, 7), new Int2(1, 2), new Int2(1, 2), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(5, 7), new Int2(3, 4), new Int2(9, 9), new Int2(1, 2), new Int2(1, 2), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(3, 4), new Int2(8, 9), new Int2(3, 4), new Int2(1, 2), new Int2(1, 2), new Int2(1, 2))
    };

    private static Dictionary<int, int> barrierTileDictionary = new Dictionary<int, int>
    {
        {2, 0}, // SingleElevated
        {1012, 1}, // LowerLeftElevated
        {1112, 2}, // LowerElevated
        {1102, 3}, // LowerRightElevated
        {12, 4}, // LeftThinElevated
        {112, 5}, // HorizontalThinElevated
        {102, 6}, // RightThinElevated
        {1000, 8}, // SingleWall
        {1010, 9}, // MiddleWall
        {1110, 10}, // MiddleWall
        {1100, 11}, // MiddleWall
        {1, 12}, // TopThinElevated
        {11, 13}, // TopLeftElevated
        {1101, 14}, // RightEdgeElevated
        {1011, 16}, // LeftEdgeElevated
        {1111, 17}, // Elevated
        {1001, 20}, // VerticalThinElevated
        {1002, 21}, // BottomThinElevated
        {111, 22}, // TopEdgeElevated
        {101, 27}, // TopRightElevated
        {1212, 1}, // LowerLeftElevated
        {1122, 3}, // LowerRightElevated
        {1222, 21}, // BottomThinElevated
        {1221, 20}, // VerticalThinElevated
        {1211, 16}, // LeftEdgeElevated
        {1121, 14}, // RightEdgeElevated
        {1020, 9}, // LeftWall
        {1021, 20}, // VerticalThinElevated
        {1022, 21}, // BottomThinElevated
        {1200, 11}, // RightWall
        {1201, 20}, // VerticalThinElevated
        {1202, 21}, // BottomThinElevated
        {1120, 10}, // MiddleWall
        {1210, 10}, // MiddleWall
        {121, 27}, // TopRightElevated
        {211, 13}, // TopLeftElevated
        {221, 12}, // TopThinElevated
        {222, 0}, // SingleElevated
        {202, 0}, // SingleElevated
        {212, 4}, // LeftThinElevated
        {201, 12}, // TopThinElevated
        {1220, 10}, // MiddleWall
        {21, 12}, // TopThinElevated
        {22, 0}, // SingleElevated
        {122, 6}, // RightThinElevated

        // TODO: These next ones are lone blocks. Make them into rocks or other obstacles.
        {0, 10},
        {10, 10},
        {100, 10},
        {110, 10},
        {120, 10},
        {200, 10},
        {20, 10},
        {210, 10},
        {220, 10}
    };
}
