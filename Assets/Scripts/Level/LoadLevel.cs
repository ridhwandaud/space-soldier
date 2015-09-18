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
    public static bool WALL_COLLIDERS_INITIALIZED = false;

    private static int WALL_LAYER = 8;

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
        WALL_COLLIDERS_INITIALIZED = false;

        Vector3 playerSpawn;
        GameObject player = GameObject.Find("Soldier");

        Tile.SetCamera();

        BasicLevelGenerator generator = new BasicLevelGenerator();
        int[,] generatedLevel = generator.GenerateLevel(0, out playerSpawn);
        setTiles(generatedLevel, playerSpawn, player);
        AStar.world = generatedLevel;
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
        Tile.SetColliderLayer(WALL_LAYER);

        for (int row = 0; row < generatedLevel.GetLength(0); row++)
        {
            for (int col = 0; col < generatedLevel.GetLength(1); col++)
            {
                Int2 tileLocation = new Int2(col, row);
                bool isWall = WALL_INDICES.Contains(generatedLevel[row, col]);
                int tileIndex = isWall ? 2 : 0;
                //int tileIndex = generatedLevel[row, col];
                int spriteTileLayerIndex = isWall ? 1 : 0;

                Tile.SetTile(tileLocation, spriteTileLayerIndex, 0, tileIndex, false);

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
        }

        WALL_COLLIDERS_INITIALIZED = true;
    }

    private bool hasAdjacentFloor(int[,] level, int x, int y)
    {
        return (x < level.GetLength(0) - 1 && FLOOR_INDICES.Contains(level[x + 1, y]))
            || (x > 0 && FLOOR_INDICES.Contains(level[x - 1, y]))
            || (y < level.GetLength(1) - 1 && FLOOR_INDICES.Contains(level[x, y + 1]))
            || (y > 0 && FLOOR_INDICES.Contains(level[x, y - 1]));
    }
}
