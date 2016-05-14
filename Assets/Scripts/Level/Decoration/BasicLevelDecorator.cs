using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelDecorator {
    public static int TilesetIndex = 2;
    private static int NumLayers = 11;

    // SpriteTile layers/sorting layers.
    public static int FloorTileLayer = 0,
                      AdditionalGrassTileLayer1 = 1,
                      AdditionalGrassTileLayer2 = 2,
                      AdditionalGrassTileLayer3 = 3,
                      AdditionalGrassTileLayer4 = 4,
                      ShoreTileLayer = 5,
                      DoodadTileLayer = 6,
                      ShadowTileLayer = 7,
                      WaterTileLayer = 8,
                      CliffTileLayer = 9,
                      TreeTileLayer = 11;

    private static int LightGrass = 0;
    private static int TreeTopLeft = 8, TreeTopRight = 9, TreeBottomLeft = 13, TreeBottomRight = 14;
    private static int TreeLeftShadow = 17, TreeRightShadow = 18;
    private static int WaterWallLeft = 72, WaterWallCenter = 81, WaterWallRight = 67,
        WaterWallSingle = 105, WaterVariation = 57;
    private static int LeftWall = 36, MiddleWall = 37, RightWall = 38;
    private static int NumTilesInTree = 4;

    public static int BaseLight = 0, BaseDark = 1, BaseLightElevated = 2, BaseDarkElevated = 3, BaseWater = 5, BaseLightTree = 6, BaseDarkTree = 7;
    public static int ElevationIndexDiff = 2;
    public static int TreeIndexDiff = 6;
    public static int DoodadChance = 1;

    private static bool PlaceShadows = true;

    private int[,] level;

	public void DecorateWorld(int[,] level, bool isBossLevel, Vector2 playerSpawn, List<Vector2> openPositions)
    {
        this.level = level;
        SetUpMap();
        LayDefaultTiles(LightGrass);
        CreateLakes();
        RemoveStragglers();
        if (!isBossLevel)
        {
            PlaceTrees(playerSpawn, openPositions);
        }

        SetGrassAndCliffs();
        DecorateShores();
        AnimateWater();
    }

    void AnimateWater()
    {
        Tile.AnimateTile(new TileInfo(TilesetIndex, WaterWallLeft), 3, 3f, AnimType.Reverse);
        Tile.AnimateTile(new TileInfo(TilesetIndex, WaterWallCenter), 3, 3f, AnimType.Reverse);
        Tile.AnimateTile(new TileInfo(TilesetIndex, WaterWallRight), 3, 3f, AnimType.Reverse);
        Tile.AnimateTile(new TileInfo(TilesetIndex, WaterWallSingle), 3, 3f, AnimType.Reverse);
        Tile.AnimateTile(new TileInfo(TilesetIndex, WaterVariation), 4, 4f);
    }

    void SetGrassAndCliffs()
    {
        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                int index = level[row, col];

                // In internal grid array, first dimension is row and second is col. When placing objects in game, first dimension
                // is x (col) and second is y (row). So they are flipped.
                Int2 tileLocation = new Int2(col, row);

                if (IsDarkGrass(index))
                {
                    Tile.SetTile(tileLocation, FloorTileLayer, TilesetIndex, CalculateGrassTile(col, row,
                        TileDictionaries.DarkGrassDictionary), false);
                }

                if (IsElevated(index))
                {
                    int cliffTile = CalculateBarrierTile(col, row);
                    Tile.SetTile(tileLocation, CliffTileLayer, TilesetIndex, cliffTile, true);
                    if (PlaceShadows && row > 0 && row > 0 && col > 0 && col < level.GetLength(1) - 1)
                    {
                        int left = IsElevated(level[row, col - 1]) ? 1 : 0;
                        int right = IsElevated(level[row, col + 1]) ? 1 : 0;
                        Tile.SetTile(new Int2(col, row - 1), ShadowTileLayer, TilesetIndex,
                            TileDictionaries.BarrierShadowDictionary[left * 10 + right], false);
                    }
                }
            }
        }
    }

    void RemoveStragglers()
    {
        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                if (IsElevated(level[row, col])
                    && (row != 0 && !IsElevated(level[row - 1, col]))
                    && (row < level.GetLength(0) - 1 && !IsElevated(level[row + 1, col])))
                {
                    level[row, col] -= ElevationIndexDiff;
                }
            }
        }
    }

    void PotentiallyPlaceDoodad(int row, int col)
    {
        if (Random.Range(0, 100) < DoodadChance)
        {
            Tile.SetTile(new Int2(col, row), DoodadTileLayer, TilesetIndex, SelectFromWeightedList(TileDictionaries.DoodadVariations), false);
        }
    }

    bool IsFloor(int index)
    {
        return index == BaseDark || index == BaseLight;
    }

    bool IsTree(int index)
    {
        return index == BaseLightTree || index == BaseDarkTree;
    }

    bool IsElevated(int index)
    {
        return index == BaseLightElevated || index == BaseDarkElevated;
    }

    void DecorateShores()
    {
        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                if (IsFloor(level[row, col]) || IsTree(level[row, col]))
                {
                    int shoreTile = CalculateShoreTile(col, row, level);
                    if (shoreTile != -1)
                    {
                        Tile.SetTile(new Int2(col, row), ShoreTileLayer, TilesetIndex, shoreTile, false);
                    }
                }
            }
        }
    }

    void PlaceTrees(Vector2 playerSpawn, List<Vector2> openPositions)
    {
        int numWalkableTiles = GetNumWalkableTiles();

        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                // Uniform random distribution. I don't like the clumping though so might explore a different
                // technique (Poisson perhaps)
                if (CanPlaceTree(row, col, playerSpawn) && Random.Range(0, 20) == 0)
                {
                    if (AttemptTreePlacement(row, col, numWalkableTiles, openPositions))
                    {
                        numWalkableTiles -= NumTilesInTree;
                    } 
                }
            }
        }
    }

    bool CanPlaceTree(int row, int col, Vector2 playerSpawn)
    {
        return row + 1 < level.GetLength(0) && col + 1 < level.GetLength(1)
            && IsFloor(level[row, col]) && IsFloor(level[row + 1, col]) && IsFloor(level[row, col + 1]) && IsFloor(level[row + 1, col + 1])
            && !(row == playerSpawn.y && col == playerSpawn.x) && !(row + 1 == playerSpawn.y && col == playerSpawn.x)
            && !(row + 1 == playerSpawn.y && col + 1 == playerSpawn.x) && !(row == playerSpawn.y && col + 1 ==  playerSpawn.x);
    }

    int GetNumWalkableTiles()
    {
        int result = 0;

        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                result += IsFloor(level[row, col]) ? 1 : 0;
            }
        }

        return result;
    }

    bool AttemptTreePlacement (int row, int col, int numWalkableTiles, List<Vector2> openPositions)
    {
        int prevBottomLeft = level[row, col], prevBottomRight = level[row, col + 1], prevTopLeft = level[row + 1, col],
            prevTopRight = level[row + 1, col + 1];

        level[row, col] += TreeIndexDiff;
        level[row, col + 1] += TreeIndexDiff;
        level[row + 1, col] += TreeIndexDiff;
        level[row + 1, col + 1] += TreeIndexDiff;

        Queue<Int2> queue = new Queue<Int2>();
        int newNumWalkableTiles = 0;

        int[,] tracker = new int[level.GetLength(0), level.GetLength(1)];

        // Find first walkable tile
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                if (IsFloor(level[x, y]))
                {
                    queue.Enqueue(new Int2(x, y));
                    newNumWalkableTiles++;
                    tracker[x, y] = 1;
                    goto End; // nested break
                }
            }
        }
        End:;

        while (queue.Count != 0)
        {
            Int2 pos = queue.Dequeue();

            if (pos.x - 1 >= 0 && IsFloor(level[pos.x - 1, pos.y]) && tracker[pos.x - 1, pos.y] == 0)
            {
                newNumWalkableTiles++;
                queue.Enqueue(new Int2(pos.x - 1, pos.y));
                tracker[pos.x - 1, pos.y] = 1;
            }
            if (pos.x + 1 < level.GetLength(0) && IsFloor(level[pos.x + 1, pos.y]) && tracker[pos.x + 1, pos.y] == 0)
            {
                newNumWalkableTiles++;
                queue.Enqueue(new Int2(pos.x + 1, pos.y));
                tracker[pos.x + 1, pos.y] = 1;
            }
            if (pos.y - 1 >= 0 && IsFloor(level[pos.x, pos.y - 1]) && tracker[pos.x, pos.y - 1] == 0)
            {
                newNumWalkableTiles++;
                queue.Enqueue(new Int2(pos.x, pos.y - 1));
                tracker[pos.x, pos.y - 1] = 1;
            }
            if (pos.y + 1 < level.GetLength(1) && IsFloor(level[pos.x, pos.y + 1]) && tracker[pos.x, pos.y + 1] == 0)
            {
                newNumWalkableTiles++;
                queue.Enqueue(new Int2(pos.x, pos.y + 1));
                tracker[pos.x, pos.y + 1] = 1;
            }
        }

        if (newNumWalkableTiles == numWalkableTiles - NumTilesInTree)
        {
            Tile.SetTile(new Int2(col, row), TreeTileLayer, TilesetIndex, TreeBottomLeft, true);
            Tile.SetTile(new Int2(col + 1, row), TreeTileLayer, TilesetIndex, TreeBottomRight, true);
            Tile.SetTile(new Int2(col, row + 1), TreeTileLayer, TilesetIndex, TreeTopLeft, false);
            Tile.SetTile(new Int2(col + 1, row + 1), TreeTileLayer, TilesetIndex, TreeTopRight, false);

            openPositions.Remove(new Vector2(col, row));
            openPositions.Remove(new Vector2(col + 1, row));
            openPositions.Remove(new Vector2(col, row + 1));
            openPositions.Remove(new Vector2(col + 1, row + 1));

            if (row - 1 >= 0)
            {
                Tile.SetTile(new Int2(col, row - 1), ShadowTileLayer, TilesetIndex, TreeLeftShadow, true);
                Tile.SetTile(new Int2(col + 1, row - 1), ShadowTileLayer, TilesetIndex, TreeRightShadow, true);
            }

            return true;
        } else
        {
            // Revert
            level[row, col] = prevBottomLeft;
            level[row, col + 1] = prevBottomRight;
            level[row + 1, col] = prevTopLeft;
            level[row + 1, col + 1] = prevTopRight;

            return false;
        }
    }

    void CreateLakes()
    {
        int[,] tracker = new int[level.GetLength(0), level.GetLength(1)];

        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                if (IsElevated(level[row, col]) && tracker[row, col] == 0)
                {
                    AttemptLakeCreation(row, col, tracker);
                }
            }
        }

        SetWaterTiles();
    }

    void AttemptLakeCreation(int x, int y, int[,] tracker)
    {
        List<Int2> islandMembers = new List<Int2>();
        bool[,] seenThisIteration = new bool[level.GetLength(0), level.GetLength(1)];
        bool touchingEdge = false;

        Queue<Int2> queue = new Queue<Int2>();
        queue.Enqueue(new Int2(x, y));
        int count = 0;
        tracker[x, y] = 1;

        while (queue.Count > 0)
        {
            Int2 pos = queue.Dequeue();
            islandMembers.Add(pos);

            if (pos.x - 1 < 0 || pos.x + 1 >= level.GetLength(0) || pos.y - 1 < 0 || pos.y + 1 >= level.GetLength(1))
            {
                touchingEdge = true;
            }

            if (pos.x - 1 >= 0 && IsElevated(level[pos.x - 1, pos.y]) && tracker[pos.x - 1, pos.y] == 0)
            {
                count++;
                queue.Enqueue(new Int2(pos.x - 1, pos.y));
                tracker[pos.x - 1, pos.y] = 1;
            }
            if (pos.x + 1 < level.GetLength(0) && IsElevated(level[pos.x + 1, pos.y]) && tracker[pos.x + 1, pos.y] == 0)
            {
                count++;
                queue.Enqueue(new Int2(pos.x + 1, pos.y));
                tracker[pos.x + 1, pos.y] = 1;
            }
            if (pos.y - 1 >= 0 && IsElevated(level[pos.x, pos.y - 1]) && tracker[pos.x, pos.y - 1] == 0)
            {
                count++;
                queue.Enqueue(new Int2(pos.x, pos.y - 1));
                tracker[pos.x, pos.y - 1] = 1;
            }
            if (pos.y + 1 < level.GetLength(1) && IsElevated(level[pos.x, pos.y + 1]) && tracker[pos.x, pos.y + 1] == 0)
            {
                count++;
                queue.Enqueue(new Int2(pos.x, pos.y + 1));
                tracker[pos.x, pos.y + 1] = 1;
            }
              
        }

        if (!touchingEdge && Random.Range(0, 3) < 2 && count > 0)
        {
            foreach (Int2 islandSquare in islandMembers)
            {
                level[islandSquare.x, islandSquare.y] = BaseWater;
            }
        }
    }

    void SetWaterTiles()
    {
        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                if (level[row, col] == BaseWater)
                {
                    Tile.SetTile(new Int2(col, row), WaterTileLayer, TilesetIndex,
                        CalculateWaterTile(row, col), true);
                }
            }
        }
    }

    int CalculateWaterTile(int row, int col)
    {
        if (level[row + 1, col] == BaseWater)
        {
            return TileDictionaries.WaterCenterTiles[Random.Range(0, TileDictionaries.WaterCenterTiles.Count)];
        } else
        {
            int lookupIndex = 10 * (level[row + 1, col - 1] == BaseWater ? 0 : 1) + (level[row + 1, col + 1] == BaseWater ? 0 : 1);
            return TileDictionaries.WaterTopDictionary[lookupIndex];
        }
    }

    int CalculateBarrierTile(int col, int row)
    {
        int top = GetTypeForBarrier(col, row + 1),
            left = GetTypeForNonTopBarrier(col - 1, row),
            right = GetTypeForNonTopBarrier(col + 1, row),
            bottom = GetTypeForNonTopBarrier(col, row - 1);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;

        int tileIndex = TileDictionaries.BarrierTileDictionary[lookupIndex];

        if ((tileIndex == LeftWall || tileIndex == MiddleWall || tileIndex == RightWall) && row + 1 < level.GetLength(0))
        {
            lookupIndex += 10000 * GetTypeForBarrier(col + 1, row + 1);
            lookupIndex += 100000 * GetTypeForBarrier(col - 1, row + 1);
            lookupIndex += 1000000;

            if (TileDictionaries.BarrierTileDictionary.ContainsKey(lookupIndex))
            {
                tileIndex = TileDictionaries.BarrierTileDictionary[lookupIndex];
            }
        }

        return tileIndex;
    }

    int CalculateShoreTile(int col, int row, int[,] level)
    {
        int top = GetTypeForTileAboveShore(col, row + 1),
            left = GetTypeForTileAdjacentToShore(col - 1, row),
            right = GetTypeForTileAdjacentToShore(col + 1, row),
            bottom = GetTypeForTileAdjacentToShore(col, row - 1);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        if (TileDictionaries.ShoreDictionary.ContainsKey(lookupIndex))
        {
            return TileDictionaries.ShoreDictionary[lookupIndex];
        }

        return -1;
    }

    int GetTypeForTileAboveShore(int col, int row)
    {
        if (row >= level.GetLength(0) || level[row, col] == BaseWater)
        {
            return 0;
        }

        bool leftWater = col + 1 >= level.GetLength(1) || level[row, col + 1] == BaseWater;
        bool rightWater = col - 1 < 0 || level[row, col - 1] == BaseWater;

        if (!leftWater && !rightWater)
        {
            return 1;
        }
        if (leftWater && !rightWater)
        {
            return 2;
        }
        if (!leftWater && rightWater)
        {
            return 3;
        }
        if (leftWater && rightWater)
        {
            return 4;
        }

        return -1; //you done fucked up
    }

    int GetTypeForTileAdjacentToShore(int col, int row)
    {
        return col < 0 || col >= level.GetLength(1) || row < 0 || row >= level.GetLength(0) || level[row, col] == BaseWater ? 0 : 1;
    }

    int GetTypeForBarrier(int col, int row)
    {
        if (col < 0 || row < 0 || col >= level.GetLength(1) || row >= level.GetLength(0))
        {
            return 1;
        }

        int index = level[row, col];
        return IsFloor(index) || IsTree(index) ? 0 : 1;
    }

    int GetTypeForNonTopBarrier(int col, int row)
    {
        if (col < 0 || row < 0 || col >= level.GetLength(1) || row >= level.GetLength(0))
        {
            return 1;
        }

        if (GetTypeForBarrier(col, row) == 0)
        {
            return 0;
        }

        // 0 = floor, 1 = elevated non-wall, 2 = wall
        return GetTypeForBarrier(col, row - 1) == 1 ? 1 : 2;
    }

    int CalculateGrassTile(int col, int row, Dictionary<int, GrassObj[]> grassDictionary)
    {
        int top = GetTypeForGrassTile(col, row, 0, 1),
            left = GetTypeForGrassTile(col, row, -1, 0),
            right = GetTypeForGrassTile(col, row, 1, 0),
            bottom = GetTypeForGrassTile(col, row, 0, -1);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        GrassObj grassObject = grassDictionary[lookupIndex][Random.Range(0, grassDictionary[lookupIndex].Length)];
        if (grassObject.DecorateEdges)
        {
            SetSurroundingGrass(col, row);
        }

        return grassObject.Index;
    }

    int GetTypeForGrassTile(int col, int row, int colOffset, int rowOffset)
    {
        int newCol = col + colOffset, newRow = row + rowOffset;
        if (newCol < 0 || newRow < 0 || newCol >= level.GetLength(1) || newRow >= level.GetLength(0))
        {
            return GrassIndexType(level[row, col]);
        }

        return GrassIndexType(level[newRow, newCol]);
    }

    void SetSurroundingGrass(int col, int row)
    {
        if (col + 1 < level.GetLength(1) && !IsDarkGrass(level[row, col + 1]))
        {
            Tile.SetTile(new Int2(col + 1, row), AdditionalGrassTileLayer1, TilesetIndex, 94, false);
        }

        if (col - 1 > 0 && !IsDarkGrass(level[row, col - 1]))
        {
            Tile.SetTile(new Int2(col - 1, row), AdditionalGrassTileLayer2, TilesetIndex, 93, false);
        }

        if (row + 1 < level.GetLength(0) && !IsDarkGrass(level[row + 1, col]))
        {
            Tile.SetTile(new Int2(col, row + 1), AdditionalGrassTileLayer3, TilesetIndex, 92, false);
        }

        if (row - 1 > 0 && !IsDarkGrass(level[row - 1, col]))
        {
            Tile.SetTile(new Int2(col, row - 1), AdditionalGrassTileLayer4, TilesetIndex, 95, false);
        }
    }

    int GrassIndexType(int index)
    {
        return IsDarkGrass(index) ? 1 : 0;
    }

    bool IsDarkGrass(int index)
    {
        return index % 2 == 1;
    }

    void SetUpMap()
    {
        Int2 mapDimensions = new Int2(level.GetLength(1), level.GetLength(0));
        Tile.NewLevel(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);

        for (int x = 0; x < NumLayers; x++)
        {
            Tile.AddLayer(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);
        }

        Tile.SetColliderLayer(GameSettings.WallLayerNumber);
    }

    void LayDefaultTiles(int defaultTileIndex)
    {
        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                Tile.SetTile(new Int2(col, row), FloorTileLayer, TilesetIndex,
                    SelectFromWeightedList(TileDictionaries.LightGrassMiddleVariations), false);
                PotentiallyPlaceDoodad(row, col);
            }
        }
    }

    int SelectFromWeightedList(List<List<int>> weightedList)
    {
        int total = 0;
        foreach (List<int> item in weightedList)
        {
            total += item[1];
        }

        int rand = Random.Range(0, total);
        int result = weightedList[0][0];

        foreach (List<int> item in weightedList)
        {
            if (rand < item[1])
            {
                result = item[0];
                break;
            }

            rand -= item[1];
        }

        return result;
    }
}
