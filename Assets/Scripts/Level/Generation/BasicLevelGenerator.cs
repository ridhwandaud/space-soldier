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
    GameObject plantBossPrefab;
    GameObject laserBearPrefab;

    enum BasicLevelSize { Small = 250, Medium = 350, Large = 450};
    enum BasicLevelDifficulty { Easy, Hard};

    private static int HardLevelThreshold = 10;
    private static int LevelOneBossThreshold = 3;

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
        plantBossPrefab = Resources.Load("PlantBoss") as GameObject;
        laserBearPrefab = Resources.Load("LaserBear") as GameObject;

        configsBySize[BasicLevelSize.Small] = smallLevelConfig;
        configsBySize[BasicLevelSize.Medium] = mediumLevelConfig;
        configsBySize[BasicLevelSize.Large] = LargeLevelConfig;

        smallLevelConfig[BasicLevelDifficulty.Easy] = smallEasyLevelConfigs;
        mediumLevelConfig[BasicLevelDifficulty.Easy] = mediumEasyLevelConfigs;
        LargeLevelConfig[BasicLevelDifficulty.Easy] = largeEasyLevelConfigs;
    }

    public int[,] GenerateLevel(int levelIndex, out Vector3 playerSpawn)
    {
        bool isBossLevel = levelIndex >= LevelOneBossThreshold;
        Vector3 bossSpawn;

        List<Vector2> openPositions;
        BasicLevelSize size = getLevelSize(isBossLevel);
        int[,] level = algorithm.ExecuteAlgorithm((int)size, out openPositions, out playerSpawn, isBossLevel, out bossSpawn);

        decorator.DecorateWorld(level, isBossLevel, playerSpawn, openPositions);
        
        if (!isBossLevel)
        {
            GameState.IsBossFight = false;
            populator.spawnEnemies(getEnemySpawnData(size, levelIndex), openPositions, playerSpawn);
        } else
        {
            GameState.IsBossFight = true;
            GameObject obj = MonoBehaviour.Instantiate(plantBossPrefab, bossSpawn, Quaternion.identity) as GameObject;
        }

        return level;
    }

    private BasicLevelSize getLevelSize(bool isBossLevel)
    {
        if (isBossLevel)
        {
            return BasicLevelSize.Medium;
        }

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
            //new SpawnData(3, 3, basicEnemyPrefab),
            new SpawnData(2, 2, footSoldierPrefab),
            new SpawnData(3, 3, gordoPrefab),
            new SpawnData(2, 2, knightPrefab),
            new SpawnData(1, 1, laserBearPrefab)
            //new SpawnData(config.gordoTrapMinMax.x, config.gordoTrapMinMax.y, gordoTrapPrefab, false),
            //new SpawnData(config.kirbyMinMax.x, config.kirbyMinMax.y, kirbyPrefab),
            //new SpawnData(config.sniperMinMax.x, config.sniperMinMax.y, sniperPrefab)
        };
    }

    public struct EnemySpawnConfig
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
    private static List<EnemySpawnConfig> smallEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(2, 3), new Int2(1, 3), new Int2(2, 3), new Int2(1, 1), new Int2(0, 0), new Int2(1, 1)),
        new EnemySpawnConfig(new Int2(2, 4), new Int2(0, 0), new Int2(3, 4), new Int2(1, 2), new Int2(0, 0), new Int2(1, 1)),
        new EnemySpawnConfig(new Int2(0, 1), new Int2(2, 2), new Int2(3, 4), new Int2(1, 2), new Int2(0, 0), new Int2(1, 1)),
    };
    private static List<EnemySpawnConfig> mediumEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(5, 6), new Int2(5, 6), new Int2(5, 6), new Int2(1, 2), new Int2(1, 1), new Int2(1, 1)),
        new EnemySpawnConfig(new Int2(6, 7), new Int2(3, 5), new Int2(8, 9), new Int2(1, 2), new Int2(1, 1), new Int2(1, 1)),
        new EnemySpawnConfig(new Int2(3, 5), new Int2(3, 5), new Int2(3, 5), new Int2(1, 2), new Int2(1, 1), new Int2(1, 1))
    };
    public static List<EnemySpawnConfig> largeEasyLevelConfigs = new List<EnemySpawnConfig>
    {
        new EnemySpawnConfig(new Int2(6, 7), new Int2(4, 5), new Int2(6, 7), new Int2(1, 2), new Int2(1, 2), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(5, 7), new Int2(3, 4), new Int2(9, 9), new Int2(1, 2), new Int2(1, 2), new Int2(1, 2)),
        new EnemySpawnConfig(new Int2(3, 4), new Int2(8, 9), new Int2(3, 4), new Int2(1, 2), new Int2(1, 2), new Int2(1, 2))
    };
}
