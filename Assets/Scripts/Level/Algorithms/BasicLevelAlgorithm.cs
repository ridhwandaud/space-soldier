using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelAlgorithm {
    public float noiseThreshold = .5f;

    private static float NOISE_CONSTANT = .15f;

    public int[,] ExecuteAlgorithm(int numTiles, out List<Vector2> openPositions, out Vector3 playerSpawn)
    {
        openPositions = new List<Vector2>();

        int[,] level = new int[numTiles * 2, numTiles * 2];

        set2DArrayDefaults(level, 2);

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

            if (level[current.x, current.y] == 2)
            {
                level[current.x, current.y] = 1;
                numTilesPlaced++;
            }

            current += getRandomDirection(new int[] { 25, 25, 25, 25 });
        }

        playerSpawn = new Vector3((startingY - bottomY + 1) * LoadLevel.TILE_SIZE, (startingX - leftX + 1) * LoadLevel.TILE_SIZE, 0);

        return cropLevel(level, leftX, rightX, topY, bottomY, openPositions);
    }

    private int getFloorTile(float x, float y, int width, int height)
    {
        float noise = Mathf.PerlinNoise(x * NOISE_CONSTANT, y * NOISE_CONSTANT);
        if (noise < noiseThreshold)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private int[,] cropLevel(int[,] levelToResize, int leftX, int rightX, int topY, int bottomY, List<Vector2> openPositions)
    {
        // Add an outer wall.
        int newWidth = rightX - leftX + 3;
        int newHeight = topY - bottomY + 3;

        int[,] result = new int[newWidth, newHeight];
        set2DArrayDefaults(result, 2);

        for (int x = 0; x < newWidth - 1; x++)
        {
            for (int y = 0; y < newHeight - 1; y++)
            {
                result[x, y] = levelToResize[x + leftX - 1, y + bottomY - 1];
                if (result[x, y] != 2)
                {
                    openPositions.Add(new Vector2(y, x));
                    result[x, y] = getFloorTile(x, y, newWidth, newHeight);
                }
            }
        }

        return result;
    }

    private void set2DArrayDefaults(int[,] arr, int defaultValue)
    {
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                arr[x, y] = defaultValue;
            }
        }
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
}
