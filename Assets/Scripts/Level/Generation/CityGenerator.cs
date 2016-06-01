using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class CityGenerator : ILevelGenerator {

    private CityAlgorithm CityAlgorithm = new CityAlgorithm();
    private CityDecorator CityDecorator = new CityDecorator();
    private BasicLevelPopulator CityPopulator = new BasicLevelPopulator();

    public int[,] GenerateLevel (int levelIndex, out Vector3 playerSpawn)
    {
        List<Rect> finalRectangles = new List<Rect>();
        List<Road> perimeterLines = new List<Road>();
        int[,] grid = CityAlgorithm.ExecuteAlgorithm(finalRectangles, perimeterLines, out playerSpawn);
        CityDecorator.GenerateBuildings(finalRectangles, grid);
        CityDecorator.DecoratePerimeters(perimeterLines, grid);
        playerSpawn.x = CityGridCreator.NormalizeX((int)playerSpawn.x);
        playerSpawn.y = CityGridCreator.NormalizeY((int)playerSpawn.y);
        PopulateMap(grid, playerSpawn);

        return grid;
    }

    // Temporary method for testing - will replace with CityPopulator
    void PopulateMap(int[,] grid, Vector2 playerSpawn)
    {
        GameObject basicEnemyPrefab = Resources.Load("Enemy") as GameObject;
        GameObject footSoldierPrefab = Resources.Load("FootSoldier") as GameObject;
        GameObject hyperScoutPrefab = Resources.Load("HyperScout") as GameObject;
        GameObject gordoPrefab = Resources.Load("Gordo") as GameObject;
        GameObject sniperPrefab = Resources.Load("Sniper") as GameObject;
        GameObject gordoTrapPrefab = Resources.Load("GordoTrap") as GameObject;
        GameObject knightPrefab = Resources.Load("Knight") as GameObject;
        GameObject kirbyPrefab = Resources.Load("Kirby") as GameObject;
        GameObject troopaPrefab = Resources.Load("Troopa") as GameObject;
        GameObject catPrefab = Resources.Load("Cat") as GameObject;
        Material wallMaterial = Resources.Load("WallMaterial") as Material;

        BasicLevelGenerator.EnemySpawnConfig config = BasicLevelGenerator.largeEasyLevelConfigs[0];

        List<SpawnData> spawnData = new List<SpawnData> {
            //new SpawnData(config.footSoldierMinMax.x, config.footSoldierMinMax.y, footSoldierPrefab),
            //new SpawnData(config.gordoMinMax.x, config.gordoMinMax.y, gordoPrefab),
            //new SpawnData(config.gordoTrapMinMax.x, config.gordoTrapMinMax.y, gordoTrapPrefab, false),
            new SpawnData(3, 4, kirbyPrefab),
            new SpawnData(4, 4, sniperPrefab),
            //new SpawnData(config.basicEnemyMinMax.x, config.basicEnemyMinMax.y, basicEnemyPrefab)
            new SpawnData(5, 5, basicEnemyPrefab),
            new SpawnData(5, 5, hyperScoutPrefab),
            new SpawnData(5, 5, footSoldierPrefab),
            new SpawnData(5, 5, catPrefab)
        };

        CityPopulator.spawnEnemies(spawnData, GetOpenSpawnPositions(grid, playerSpawn), playerSpawn);
        SetWallMaterials(wallMaterial);
    }

    void SetWallMaterials(Material wallMaterial)
    {
        CityDecorator.Buildings.ForEach(b => {
            for (int row = 0; row < b.HighRiseTiles.GetLength(0); row++)
            {
                for (int col = 0; col < b.HighRiseTiles.GetLength(1); col++)
                {
                    Tile.SetTileMaterial(Building.TilesetIndex, b.HighRiseTiles[row, col], wallMaterial);
                }
            }
        });
    }

    List<Vector2> GetOpenSpawnPositions(int[,] grid, Vector2 playerSpawn)
    {
        List<Vector2> openSpawnPositions = new List<Vector2>();

        Queue<Int2> q = new Queue<Int2>();

        q.Enqueue(new Int2((int)playerSpawn.x, (int)playerSpawn.y));
        bool[,] seen = new bool[grid.GetLength(0), grid.GetLength(1)];
        seen[(int)playerSpawn.y, (int)playerSpawn.x] = true;

        while(q.Count > 0)
        {
            Int2 curr = q.Dequeue();
            openSpawnPositions.Add(new Vector2(curr.x, curr.y));

            if (curr.x - 1 >= 0 && !seen[curr.y, curr.x - 1] && IsWalkable(grid[curr.y, curr.x - 1]))
            {
                q.Enqueue(new Int2(curr.x - 1, curr.y));
                seen[curr.y, curr.x - 1] = true;
            }
            if (curr.x + 1 < grid.GetLength(1) && !seen[curr.y, curr.x + 1] && IsWalkable(grid[curr.y, curr.x + 1]))
            {
                q.Enqueue(new Int2(curr.x + 1, curr.y));
                seen[curr.y, curr.x + 1] = true;
            }
            if (curr.y - 1 >= 0 && !seen[curr.y - 1, curr.x] && IsWalkable(grid[curr.y - 1, curr.x]))
            {
                q.Enqueue(new Int2(curr.x, curr.y - 1));
                seen[curr.y - 1, curr.x] = true;
            }
            if (curr.y + 1 < grid.GetLength(0) && !seen[curr.y + 1, curr.x] && IsWalkable(grid[curr.y + 1, curr.x]))
            {
                q.Enqueue(new Int2(curr.x, curr.y + 1));
                seen[curr.y + 1, curr.x] = true;
            }
        }

        return openSpawnPositions;
    }

    bool IsWalkable(int index)
    {
        return index == CityGridCreator.DefaultWalkableIndex || index == CityGridCreator.GridArrayRoadIndex;
    }
}
