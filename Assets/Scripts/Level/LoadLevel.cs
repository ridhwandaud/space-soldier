using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using System;
using System.Text;

public class LoadLevel : MonoBehaviour {

    public static int numTiles = 10;

    private static int WALL_INDEX = 0;
    private static int FLOOR_INDEX = 1;
    private static float TILE_SIZE = 2f;

    private Vector3 playerSpawn;
    private GameObject player;
    private List<Vector2> potentialEnemyPositions;
    private EnemyPopulationCalculator enemyPopulationCalculator;

	void Awake () {
        player = GameObject.Find("Soldier");
        enemyPopulationCalculator = GetComponent<EnemyPopulationCalculator>();

        // Difference between List and ArrayList?
        potentialEnemyPositions = new List<Vector2>();
        Tile.SetCamera();

        int[,] generatedLevel = GenerateLevel();
        setTiles(generatedLevel);
        AStar.world = generatedLevel;
        print("world dimensions: " + AStar.world.GetLength(0) + ", " + AStar.world.GetLength(1));
        String str = "";
        for (int x = 0; x < AStar.world.GetLength(0); x++)
        {
            for (int y = 0; y < AStar.world.GetLength(1); y++)
                str += AStar.world[x, y] + ", ";
            str += "\n";
        }
        print(str);
        EnemySpawner.spawnEnemies(enemyPopulationCalculator.getEnemyData(0), potentialEnemyPositions);
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

        String str = "";

        for (int row = 0; row < generatedLevel.GetLength(0); row++)
        {
            for (int col = 0; col < generatedLevel.GetLength(1); col++)
            {
                Int2 tileLocation = new Int2(col, row);
                bool isWall = generatedLevel[row, col] == WALL_INDEX;
                int tileIndex = isWall ? 2 : 0;
                int layerIndex = isWall ? 1 : 0;

                Tile.SetTile(tileLocation, layerIndex, 0, tileIndex, false);

                if (isWall)
                {
                    str += "w, ";
                }
                else
                {
                    str += "f, ";
                }

                if (isWall && hasAdjacentFloor(generatedLevel, row, col))
                {
                    Tile.SetCollider(tileLocation, 1, true);
                }
            }
            str += "\n";
        }

        print(str);

        StartCoroutine("SetColliders");
        player.GetComponent<Rigidbody2D>().position = playerSpawn;
    }

    IEnumerator SetColliders()
    {
        yield return new WaitForEndOfFrame();

        PolygonCollider2D[] polygonColliders = GameObject.Find("SpriteTileColliders").GetComponentsInChildren<PolygonCollider2D>();
        foreach (PolygonCollider2D collider in polygonColliders)
        {
            //collider.isTrigger = true;
            collider.gameObject.AddComponent(Type.GetType("WallCollision"));
        }

    }

    int[,] GenerateLevel()
    {
        int[,] level = new int[numTiles * 2, numTiles * 2];
        int startingX = numTiles, startingY = numTiles;

        Int2 current = new Int2(startingX, startingY);
        Int2 directionLastMoved = new Int2(0, 0);
        int numTilesPlaced = 0;

        // For resizing the level
        int leftX = current.x;
        int rightX = current.x;
        int topY = current.y;
        int bottomY = current.y;

        while (numTilesPlaced < numTiles)
        {
            leftX = current.x < leftX ? current.x : leftX;
            rightX = current.x > rightX ? current.x : rightX;
            topY = current.y > topY ? current.y : topY;
            bottomY = current.y < bottomY ? current.y : bottomY;

            if (level[current.x, current.y] == 0)
            {
                level[current.x, current.y] = 1;
                numTilesPlaced++;
            }

            current += getRandomDirection(new int[] {25, 25, 25, 25});
        }

        playerSpawn = new Vector3((startingY - bottomY + 1) * TILE_SIZE, (startingX - leftX + 1) * TILE_SIZE, 0);

        return cropLevel(level, leftX, rightX, topY, bottomY);
    }

    private int[,] cropLevel(int[,] levelToResize, int leftX, int rightX, int topY, int bottomY)
    {
        // Add an outer wall.
        int newWidth = rightX - leftX + 3;
        int newHeight = topY - bottomY + 3;

        int[,] result = new int[newWidth, newHeight];

        for (int x = 1; x < newWidth - 1; x++)
        {
            for (int y = 1; y < newHeight - 1; y++)
            {
                result[x, y] = levelToResize[x + leftX - 1, y + bottomY - 1];
                if (result[x, y] == 1)
                {
                    potentialEnemyPositions.Add(new Vector2(y * TILE_SIZE, x * TILE_SIZE));
                }
            }
        }

        return result;
    }

    private Int2 getRandomDirection(int[] directionWeights)
    {
        int directionWeightSum = 0;
        foreach (int weight in directionWeights)
        {
            directionWeightSum += weight;
        }

        int random = UnityEngine.Random.Range(0, directionWeightSum);

        if (random < directionWeights[0])
            return Int2.up;
        else if (random < directionWeights[0] + directionWeights[1])
            return Int2.down;
        else if (random < directionWeights[0] + directionWeights[1] + directionWeights[2])
            return Int2.left;
        else
            return Int2.right;
    }

    private bool hasAdjacentFloor(int[,] level, int x, int y)
    {
        return (x < level.GetLength(0) - 1 && level[x + 1, y] == FLOOR_INDEX)
            || (x > 0 && level[x - 1, y] == FLOOR_INDEX)
            || (y < level.GetLength(1) - 1 && level[x, y + 1] == FLOOR_INDEX)
            || (y > 0 && level[x, y - 1] == FLOOR_INDEX);
    }
}
