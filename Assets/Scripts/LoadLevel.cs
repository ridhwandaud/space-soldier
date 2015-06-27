using UnityEngine;
using System.Collections;
using SpriteTile;
using System;
using System.Text;

public class LoadLevel : MonoBehaviour {

    public TextAsset level;

    public static int NUM_TILES = 500;

    private static int WALL_INDEX = 0;
    private static int FLOOR_INDEX = 1;

	void Awake () {
        Tile.SetCamera();

        setTiles(GenerateLevel());
	}

    void setTiles(int[,] generatedLevel)
    {
        Int2 mapDimensions = new Int2(generatedLevel.GetLength(0), generatedLevel.GetLength(1));

        // create level
        Tile.NewLevel(mapDimensions, 0, 1, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, 1, 0, LayerLock.None);

        // set sorting layers
        Tile.SetLayerSorting(0, 0);
        Tile.SetLayerSorting(1, 1);

        for (int x = 0; x < generatedLevel.GetLength(0); x++)
        {
            for (int y = 0; y < generatedLevel.GetLength(1); y++)
            {
                Int2 tileLocation = new Int2(x, y);
                bool isWall = generatedLevel[x, y] == WALL_INDEX;
                int tileIndex = isWall ? 2 : 0;
                int layerIndex = isWall ? 1 : 0;

                Tile.SetTile(tileLocation, layerIndex, 0, tileIndex, false);

                if (isWall && hasAdjacentFloor(generatedLevel, x, y))
                {
                    Tile.SetCollider(tileLocation, 1, true);
                }
            }
        }

        StartCoroutine("SetColliders");
    }

    IEnumerator SetColliders()
    {
        yield return new WaitForEndOfFrame();

        PolygonCollider2D[] polygonColliders = GameObject.Find("SpriteTileColliders").GetComponentsInChildren<PolygonCollider2D>();
        foreach (PolygonCollider2D collider in polygonColliders)
        {
            collider.isTrigger = true;
            collider.gameObject.AddComponent(Type.GetType("WallCollision"));
        }

    }

    int[,] GenerateLevel()
    {
        int[,] level = new int[NUM_TILES * 2, NUM_TILES * 2];

        Int2 current = new Int2(NUM_TILES, NUM_TILES);
        Int2 directionLastMoved = new Int2(0, 0);
        int numTilesPlaced = 0;

        // For resizing the level
        int leftX = current.x;
        int rightX = current.x;
        int topY = current.y;
        int bottomY = current.y;

        while (numTilesPlaced < NUM_TILES)
        {
            leftX = current.x < leftX ? current.x : leftX;
            rightX = current.x > rightX ? current.x : rightX;
            topY = current.y < topY ? current.y : topY;
            bottomY = current.y > bottomY ? current.y : bottomY;

            if (level[current.x, current.y] == 0)
            {
                level[current.x, current.y] = 1;
                numTilesPlaced++;
            }

            current += getRandomDirection(new int[] {25, 25, 25, 25});
        }

        return cropLevel(level, leftX, rightX, topY, bottomY);
    }

    private int[,] cropLevel(int[,] levelToResize, int leftX, int rightX, int topY, int bottomY)
    {
        // Add an outer wall.
        int newWidth = rightX - leftX + 3;
        int newHeight = bottomY - topY + 3;

        int[,] result = new int[newWidth, newHeight];

        for (int x = 1; x < newWidth - 1; x++)
        {
            for (int y = 1; y < newHeight - 1; y++)
            {
                result[x, y] = levelToResize[x + leftX - 1, y + topY - 1];
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
