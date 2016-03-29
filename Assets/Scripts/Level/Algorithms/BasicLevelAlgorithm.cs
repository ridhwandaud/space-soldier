using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelAlgorithm {
    public float noiseThreshold = .5f;

    private static float NoiseConstant = .15f;
    private static int BossRoomStampSize = 15;
    private static int BossLevelCorridorTiles = 120;
    private static int BossCorridorStampSize = 2;
    private static int NormalStampSize = 1;

    private static List<Direction> CorridorDirections = new List<Direction> { Direction.Left, Direction.Right,
        Direction.Up, Direction.Down };

    public int[,] ExecuteAlgorithm(int numTiles, out List<Vector2> openPositions, out Vector3 playerSpawn, bool isBossLevel, out Vector3 bossSpawn)
    {
        Vector3 bossSpawnGridPos = Vector3.zero;
        openPositions = new List<Vector2>();

        int[,] level = new int[numTiles * 2, numTiles * 2];

        set2DArrayDefaults(level, 2);

        int startingX = numTiles, startingY = numTiles;

        Int2 current = new Int2(startingX, startingY);
        Int2 directionLastMoved = new Int2(0, 0);
        Int2 mostRecentDir = new Int2(0, 0);
        int numTilesPlaced = 0;
        Direction directionBias = Direction.None;

        if (isBossLevel)
        {
            directionBias = CorridorDirections[Random.Range(0, CorridorDirections.Count)];
        }

        // For resizing the level
        int leftX = current.x;
        int rightX = current.x;
        int topY = current.y;
        int bottomY = current.y;

        while (numTilesPlaced < numTiles)
        {

            int maxRowOffset = isBossLevel ? (numTilesPlaced >= BossLevelCorridorTiles ? BossRoomStampSize : BossCorridorStampSize) : NormalStampSize;
            int maxColOffset = isBossLevel ? (numTilesPlaced >= BossLevelCorridorTiles ? BossRoomStampSize : BossCorridorStampSize) : NormalStampSize;
            int rowIncrement = isBossLevel && numTilesPlaced >= BossLevelCorridorTiles && directionBias == Direction.Right ? -1 : 1;
            int colIncrement = isBossLevel && numTilesPlaced >= BossLevelCorridorTiles && directionBias == Direction.Up ? -1 : 1;

            if (isBossLevel && numTilesPlaced >= BossLevelCorridorTiles)
            {
                bossSpawnGridPos = new Vector3((current.x + rowIncrement * (BossRoomStampSize / 2)),
                    (current.y + colIncrement * (BossRoomStampSize / 2)));
            }

            for (int rowOffset = 0; Mathf.Abs(rowOffset) < maxRowOffset; rowOffset += rowIncrement)
            {
                for (int colOffset = 0; Mathf.Abs(colOffset) < maxColOffset; colOffset += colIncrement)
                {
                    numTilesPlaced += level[current.x + rowOffset, current.y + colOffset] == 2 ? 1 : 0;
                    level[current.x + rowOffset, current.y + colOffset] = 1;

                    leftX = current.x + rowOffset < leftX ? current.x + rowOffset : leftX;
                    rightX = current.x + rowOffset > rightX ? current.x + rowOffset : rightX;
                    topY = current.y + colOffset > topY ? current.y + colOffset : topY;
                    bottomY = current.y + colOffset < bottomY ? current.y + colOffset : bottomY;
                }
            }

            bool creatingBossCorridor = isBossLevel && numTilesPlaced < BossLevelCorridorTiles;

            current += getRandomDirection(new int[] {
                creatingBossCorridor && directionBias == Direction.Right ? 75 : 25,
                creatingBossCorridor && directionBias == Direction.Left ? 75 : 25,
                creatingBossCorridor && directionBias == Direction.Up ? 75 : 25,
                creatingBossCorridor && directionBias == Direction.Down ? 75 : 25
            });
        }

        playerSpawn = new Vector3((startingY - bottomY + 1) * GameSettings.TileSize, (startingX - leftX + 1) * GameSettings.TileSize, 0);
        bossSpawn = new Vector3((bossSpawnGridPos.y - bottomY + 1) * GameSettings.TileSize, (bossSpawnGridPos.x - leftX + 1) * GameSettings.TileSize, 0);

        return cropLevel(level, leftX, rightX, topY, bottomY, openPositions);
    }

    private int getFloorTile(float x, float y, int width, int height)
    {
        float noise = Mathf.PerlinNoise(x * NoiseConstant, y * NoiseConstant);
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

        int random = Random.Range(0, directionWeightSum);

        if (random < directionWeights[0])
            return Int2.up;
        else if (random < directionWeights[0] + directionWeights[1])
            return Int2.down;
        else if (random < directionWeights[0] + directionWeights[1] + directionWeights[2])
            return Int2.left;
        else
            return Int2.right;
    }

    private enum Direction
    {
        None, Left, Right, Up, Down
    }
}
