using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using System;
using System.Text;

public class LoadLevel : MonoBehaviour {
    HashSet<int> WALL_INDICES = new HashSet<int>() {0};
    HashSet<int> FLOOR_INDICES = new HashSet<int>() {1};

    public static float TILE_SIZE = 2f;
    public static LoadLevel instance = null;

    private List<LevelType> levelTypes;
    private int level = 0;

	void Awake () {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        InitLevel();
	}

    void OnLevelWasLoaded(int index)
    {
        level++;
        InitLevel();
    }

    void InitLevel()
    {
        Vector3 playerSpawn;
        List<Vector2> potentialEnemyPositions;
        GameObject player = GameObject.Find("Soldier");

        // Eventually there will be multiple level types and the level type will be randomly selected.
        levelTypes = new List<LevelType>();
        levelTypes.Add(new LevelType(new BasicLevelGenerator(), new BasicLevelPopulator()));

        Tile.SetCamera();

        int[,] generatedLevel = levelTypes[0].getLevelGenerator().GenerateLevel(0,
            out potentialEnemyPositions, out playerSpawn);
        setTiles(generatedLevel, playerSpawn, player);
        AStar.world = generatedLevel;

        levelTypes[0].getLevelPopulator().populateLevel(0, potentialEnemyPositions);
    }

    void setTiles(int[,] generatedLevel, Vector3 playerSpawn, GameObject player)
    {
        Int2 mapDimensions = new Int2(generatedLevel.GetLength(1), generatedLevel.GetLength(0));

        // create level
        Tile.NewLevel(mapDimensions, 0, TILE_SIZE, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, TILE_SIZE, 0, LayerLock.None);

        // set sorting layers
        Tile.SetLayerSorting(0, 0);
        Tile.SetLayerSorting(1, 1);

        // set collider layer so that walls can be detected by raycasting
        Tile.SetColliderLayer(8);

        for (int row = 0; row < generatedLevel.GetLength(0); row++)
        {
            for (int col = 0; col < generatedLevel.GetLength(1); col++)
            {
                Int2 tileLocation = new Int2(col, row);
                bool isWall = WALL_INDICES.Contains(generatedLevel[row, col]);
                int tileIndex = isWall ? 2 : 0;
                int layerIndex = isWall ? 1 : 0;

                Tile.SetTile(tileLocation, layerIndex, 0, tileIndex, false);

                if (isWall && hasAdjacentFloor(generatedLevel, row, col))
                {
                    Tile.SetCollider(tileLocation, 1, true);
                }
            }
        }

        StartCoroutine("ConfigureColliders");
        player.GetComponent<Rigidbody2D>().position = playerSpawn;
    }

    IEnumerator ConfigureColliders()
    {
        yield return new WaitForEndOfFrame();

        PolygonCollider2D[] polygonColliders = GameObject.Find("SpriteTileColliders").GetComponentsInChildren<PolygonCollider2D>();
        foreach (PolygonCollider2D collider in polygonColliders)
        {
            collider.tag = "Wall";
            //A: collider.sharedMaterial = (PhysicsMaterial2D) Resources.Load("FrictionlessMaterial");
        }
    }

    private bool hasAdjacentFloor(int[,] level, int x, int y)
    {
        return (x < level.GetLength(0) - 1 && FLOOR_INDICES.Contains(level[x + 1, y]))
            || (x > 0 && FLOOR_INDICES.Contains(level[x - 1, y]))
            || (y < level.GetLength(1) - 1 && FLOOR_INDICES.Contains(level[x, y + 1]))
            || (y > 0 && FLOOR_INDICES.Contains(level[x, y - 1]));
    }
}
