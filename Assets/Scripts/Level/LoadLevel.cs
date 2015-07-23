using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using System;
using System.Text;

public class LoadLevel : MonoBehaviour {

    public GameObject player;

    HashSet<int> WALL_INDICES = new HashSet<int>() {0};
    HashSet<int> FLOOR_INDICES = new HashSet<int>() {1};

    public static float TILE_SIZE = 2f;

    private Vector3 playerSpawn;
    private List<Vector2> potentialEnemyPositions;
    private List<LevelType> levelTypes;

	void Awake () {
        levelTypes = new List<LevelType>();
        levelTypes.Add(new LevelType(new BasicLevelGenerator(), new BasicLevelPopulator()));
     
        Tile.SetCamera();

        int[,] generatedLevel = levelTypes[0].getLevelGenerator().GenerateLevel(0, 
            out potentialEnemyPositions, out playerSpawn);
        setTiles(generatedLevel);
        AStar.world = generatedLevel;

        levelTypes[0].getLevelPopulator().populateLevel(0, potentialEnemyPositions);
	}

    void setTiles(int[,] generatedLevel)
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
