using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class LoadLevel : MonoBehaviour {
    public static HashSet<int> WallIndices = new HashSet<int>() {2};
    public static HashSet<int> FloorIndices = new HashSet<int>() {0, 1};

    public static float TileSize = 2f;
    public static LoadLevel instance = null;
    public static bool WallCollidersInitialized = false;

    private static int WallLayer = 8;

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
        InitLevel();
    }

    void InitLevel()
    {
        Debug.Log("You have killed " + GameState.NumEnemiesKilled + " enemies.");

        Vector3 playerSpawn;
        GameObject player = GameObject.Find("Soldier");

        Tile.SetCamera();

        BasicLevelGenerator generator = new BasicLevelGenerator();
        int[,] generatedLevel = generator.GenerateLevel(GameState.LevelIndex, out playerSpawn);
        setTiles(generatedLevel, playerSpawn, player);

        AStar.world = generatedLevel;
    }

    void setTiles(int[,] generatedLevel, Vector3 playerSpawn, GameObject player)
    {
        Int2 mapDimensions = new Int2(generatedLevel.GetLength(1), generatedLevel.GetLength(0));

        // create level
        Tile.NewLevel(mapDimensions, 0, TileSize, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, TileSize, 0, LayerLock.None);

        // set sorting layers
        Tile.SetLayerSorting(0, 0);
        Tile.SetLayerSorting(1, 1);

        // set collider layer so that walls can be detected by raycasting
        Tile.SetColliderLayer(WallLayer);

        for (int row = 0; row < generatedLevel.GetLength(0); row++)
        {
            for (int col = 0; col < generatedLevel.GetLength(1); col++)
            {
                Int2 tileLocation = new Int2(col, row);
                bool isWall = WallIndices.Contains(generatedLevel[row, col]);
                int tileIndex = generatedLevel[row, col];
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

    public static IEnumerator ConfigureColliders()
    {
        yield return new WaitForEndOfFrame();

        PolygonCollider2D[] polygonColliders = GameObject.Find("SpriteTileColliders").GetComponentsInChildren<PolygonCollider2D>();
        foreach (PolygonCollider2D collider in polygonColliders)
        {
            collider.tag = "Wall";
        }

        GameState.WallCollidersInitialized = true;
    }

    private bool hasAdjacentFloor(int[,] level, int x, int y)
    {
        return (x < level.GetLength(0) - 1 && FloorIndices.Contains(level[x + 1, y]))
            || (x > 0 && FloorIndices.Contains(level[x - 1, y]))
            || (y < level.GetLength(1) - 1 && FloorIndices.Contains(level[x, y + 1]))
            || (y > 0 && FloorIndices.Contains(level[x, y - 1]));
    }
}
