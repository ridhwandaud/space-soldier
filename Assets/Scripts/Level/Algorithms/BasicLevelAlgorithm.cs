using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelAlgorithm {
    public float noiseThreshold = .5f;

    private static float NoiseConstant = .2f;
    private static int BossRoomStampSize = 15;
    private static int BossLevelCorridorTiles = 100;
    private static int BossCorridorStampSize = 2;
    private static int NormalStampSize = 1;
    private static int LevelPadding = 10;
    private static int DefaultArrayValue = 2; // Elevated
    private Vector2 NoiseOffset;

    private static List<Direction> CorridorDirections = new List<Direction> { Direction.Left, Direction.Right,
        Direction.Up, Direction.Down };

    // When dealing with tiles, x is always row and y is always column. Think of it as dimension 0 and dimension 1 instead of x and y.
    public int[,] ExecuteAlgorithm(int numTiles, out List<Vector2> openPositions, out Vector3 playerSpawn, bool isBossLevel, out Vector3 bossSpawn)
    {
        NoiseOffset = new Vector2(Random.Range(0, 100), Random.Range(0, 100));
        Vector3 bossSpawnGridPos = Vector3.zero;
        openPositions = new List<Vector2>();

        int[,] level = new int[numTiles * 2, numTiles * 2];

        set2DArrayDefaults(level);

        Int2 current = new Int2(numTiles, numTiles);
        Int2 mostRecentDir = new Int2(0, 0);
        // Consider refactoring the boss room logic into a separate method entirely since it is starting
        // to deviate more and more.
        Int2 bossRoomWallEntrance = new Int2(0, 0);
        int numTilesPlaced = 0;
        Direction directionBias = Direction.None;

        if (isBossLevel)
        {
            directionBias = CorridorDirections[Random.Range(0, CorridorDirections.Count)];
            Debug.Log(directionBias.ToString());
        }

        // For resizing the level
        int leftCol = current.x;
        int rightCol = current.x;
        int topRow = current.y;
        int bottomRow = current.y;

        while (numTilesPlaced < numTiles)
        {
            int maxRowOffset = isBossLevel ? (numTilesPlaced >= BossLevelCorridorTiles ? BossRoomStampSize : BossCorridorStampSize) : NormalStampSize;
            int maxColOffset = isBossLevel ? (numTilesPlaced >= BossLevelCorridorTiles ? BossRoomStampSize : BossCorridorStampSize) : NormalStampSize;
            int rowIncrement = isBossLevel && numTilesPlaced >= BossLevelCorridorTiles && directionBias == Direction.Up ? 1 : -1;
            int colIncrement = isBossLevel && numTilesPlaced >= BossLevelCorridorTiles && directionBias == Direction.Right ? 1 : -1;

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

                    if (isBossLevel && numTilesPlaced == BossLevelCorridorTiles)
                    {
                        bossRoomWallEntrance = new Int2(current.x + rowOffset, current.y + colOffset);
                    }

                    leftCol = current.y + colOffset < leftCol ? current.y + colOffset : leftCol;
                    rightCol = current.y + colOffset > rightCol ? current.y + colOffset : rightCol;
                    topRow = current.x + rowOffset > topRow ? current.x + rowOffset : topRow;
                    bottomRow = current.x + rowOffset < bottomRow ? current.x + rowOffset : bottomRow;
                }
            }

            bool creatingBossCorridor = isBossLevel && numTilesPlaced < BossLevelCorridorTiles;

            if (isBossLevel && !creatingBossCorridor && directionBias == Direction.Right) { }

            current += getRandomDirection(new int[] {
                isBossLevel ? (directionBias == Direction.Right && creatingBossCorridor ? 75
                    : (!creatingBossCorridor && directionBias == Direction.Left ? 0 : 25)) : 25,
                isBossLevel ? (directionBias == Direction.Left && creatingBossCorridor ? 75
                    : (!creatingBossCorridor && directionBias == Direction.Right ? 0 : 25)) : 25,
                isBossLevel ? (directionBias == Direction.Up && creatingBossCorridor ? 75
                    : (!creatingBossCorridor && directionBias == Direction.Down ? 0 : 25)) : 25,
                isBossLevel ? (directionBias == Direction.Down && creatingBossCorridor ? 75
                    : (!creatingBossCorridor && directionBias == Direction.Down ? 0 : 25)) : 25
            });
        }

        playerSpawn = new Vector3((numTiles + LevelPadding - leftCol + 1) * GameSettings.TileSize, (numTiles + LevelPadding - bottomRow + 1)
            * GameSettings.TileSize, 0);
        bossSpawn = new Vector3((bossSpawnGridPos.y + LevelPadding - leftCol + 1) * GameSettings.TileSize,
            (bossSpawnGridPos.x + LevelPadding - bottomRow + 1) * GameSettings.TileSize, 0);
        bossRoomWallEntrance.x = bossRoomWallEntrance.x + LevelPadding - bottomRow + 1;
        bossRoomWallEntrance.y = bossRoomWallEntrance.y + LevelPadding - leftCol + 1;
        int[,] croppedLevel = cropLevel(level, leftCol, rightCol, topRow, bottomRow, openPositions);

        if (isBossLevel)
        {
            BossLevelWallBuilder.Initialize(bossRoomWallEntrance, croppedLevel, directionBias);
        }

        return croppedLevel;
    }

    private List<Vector2> getBlockedTiles(Direction dir, int[,] level)
    {
        return null;
    }

    private int getFloorTile(float x, float y, int width, int height)
    {
        float noise = Mathf.PerlinNoise(x * NoiseConstant + NoiseOffset.x, y * NoiseConstant + NoiseOffset.y);
        if (noise < noiseThreshold)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    // Elevated tile index contains information on what the underlying floor tile is.
    private int getElevatedTile(float x, float y, int width, int height)
    {
        float noise = Mathf.PerlinNoise(x * NoiseConstant + NoiseOffset.x, y * NoiseConstant + NoiseOffset.y);
        if (noise < noiseThreshold)
        {
            return 3;
        }
        else
        {
            return 2;
        }
    }

    private int[,] cropLevel(int[,] levelToResize, int leftCol, int rightCol, int topRow, int bottomRow, List<Vector2> openPositions)
    {
        // Add an outer wall.
        int newWidth = rightCol - leftCol + LevelPadding * 2;
        int newHeight = topRow - bottomRow + LevelPadding * 2;

        int[,] result = new int[newHeight, newWidth];
        set2DArrayDefaults(result);

        for (int x = 0; x < newHeight - 1; x++)
        {
            for (int y = 0; y < newWidth - 1; y++)
            {
                result[x, y] = levelToResize[x - LevelPadding + bottomRow - 1, y - LevelPadding + leftCol - 1];
                if (x < LevelPadding || y < LevelPadding || result[x, y] == DefaultArrayValue)
                {
                    result[x, y] = getElevatedTile(x, y, newWidth, newHeight);
                } else
                {
                    openPositions.Add(new Vector2(y, x));
                    result[x, y] = getFloorTile(x, y, newWidth, newHeight);
                }
            }
        }

        return result;
    }

    private void set2DArrayDefaults(int[,] arr)
    {
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                arr[x, y] = DefaultArrayValue;
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
            return Int2.right;
        else
            return Int2.left;
    }

    public enum Direction
    {
        None, Left, Right, Up, Down
    }
}
