using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelDecorator {
    public static int TilesetIndex = 3;
    private static int NumLayers = 8;

    // SpriteTile layers/sorting layers.
    public static int FloorLayer = 0,
                      AdditionalGrassLayer1 = 1,
                      AdditionalGrassLayer2 = 2,
                      AdditionalGrassLayer3 = 3,
                      AdditionalGrassLayer4 = 4,
                      ShoreLayer = 5,
                      DoodadLayer = 6,
                      ShadowLayer = 7,
                      CliffLayer = 8;

    private static int LightGrass = 0;
    private static int TreeTopLeft = 8, TreeTopRight = 9, TreeBottomLeft = 13, TreeBottomRight = 14;
    private static int WaterWallLeft = 72, WaterWallCenter = 81, WaterWallRight = 67,
        WaterWallSingle = 105, WaterVariation = 57;
    private static int NumTilesInTree = 4;

    public static int BaseLight = 0, BaseDark = 1, BaseLightElevated = 2, BaseDarkElevated = 3, BaseWater = 5, BaseLightTree = 6, BaseDarkTree = 7;
    public static int ElevationIndexDiff = 2;
    public static int TreeIndexDiff = 6;
    public static int DoodadChance = 1;

    private static bool PlaceShadows = true;

    private int[,] level;

	public void DecorateWorld(int[,] level)
    {
        this.level = level;
        SetUpMap();
        LayDefaultTiles(LightGrass);
        CreateLakes();
        RemoveStragglers();
        PlaceTrees();
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
                    Tile.SetTile(tileLocation, FloorLayer, TilesetIndex, CalculateGrassTile(col, row,
                        TileDictionaries.DarkGrassDictionary), false);
                }

                if (IsElevated(index))
                {
                    int cliffTile = CalculateBarrierTile(col, row);
                    Tile.SetTile(tileLocation, CliffLayer, TilesetIndex, cliffTile, true);
                    if (PlaceShadows && row > 0 && row > 0 && col > 0 && col < level.GetLength(1) - 1)
                    {
                        int left = IsElevated(level[row, col - 1]) ? 1 : 0;
                        int right = IsElevated(level[row, col + 1]) ? 1 : 0;
                        Tile.SetTile(new Int2(col, row - 1), ShadowLayer, TilesetIndex,
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
            Tile.SetTile(new Int2(col, row), DoodadLayer, TilesetIndex, SelectFromWeightedList(TileDictionaries.DoodadVariations), false);
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
                        Tile.SetTile(new Int2(col, row), ShoreLayer, TilesetIndex, shoreTile, false);
                    }
                }
            }
        }
    }

    void PlaceTrees()
    {
        int numWalkableTiles = GetNumWalkableTiles();

        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                // Uniform random distribution. I don't like the clumping though so might explore a different
                // technique (Poisson perhaps)
                if (CanPlaceTree(row, col) && Random.Range(0, 20) == 0)
                {
                    if (AttemptTreePlacement(row, col, numWalkableTiles))
                    {
                        numWalkableTiles -= NumTilesInTree;
                    } 
                }
            }
        }
    }

    bool CanPlaceTree(int row, int col)
    {
        return row + 1 < level.GetLength(0) && col + 1 < level.GetLength(1)
            && IsFloor(level[row, col]) && IsFloor(level[row + 1, col]) && IsFloor(level[row, col + 1]) && IsFloor(level[row + 1, col + 1]);
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

    bool AttemptTreePlacement (int row, int col, int numWalkableTiles)
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
            Tile.SetTile(new Int2(col, row), CliffLayer, TilesetIndex, TreeBottomLeft, true);
            Tile.SetTile(new Int2(col + 1, row), CliffLayer, TilesetIndex, TreeBottomRight, true);
            Tile.SetTile(new Int2(col, row + 1), CliffLayer, TilesetIndex, TreeTopLeft, true);
            Tile.SetTile(new Int2(col + 1, row + 1), CliffLayer, TilesetIndex, TreeTopRight, true);

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
            seenThisIteration[pos.x, pos.y] = true;

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
                Tile.SetTile(new Int2(islandSquare.y, islandSquare.x), CliffLayer, TilesetIndex,
                    CalculateWaterTile(islandSquare.x, islandSquare.y, seenThisIteration), true);
            }
        }
    }

    int CalculateWaterTile(int row, int col, bool[,] seenThisIteration)
    {
        if (seenThisIteration[row + 1, col])
        {
            return TileDictionaries.WaterCenterTiles[Random.Range(0, TileDictionaries.WaterCenterTiles.Count)];
        } else
        {
            int lookupIndex = 10 * (seenThisIteration[row + 1, col - 1] ? 0 : 1) + (seenThisIteration[row + 1, col + 1] ? 0 : 1);
            return TileDictionaries.WaterTopDictionary[lookupIndex];
        }
    }

    int CalculateBarrierTile(int x, int y)
    {
        int top = GetTypeForBarrier(x, y + 1),
            left = GetTypeForNonTopBarrier(x - 1, y),
            right = GetTypeForNonTopBarrier(x + 1, y),
            bottom = GetTypeForNonTopBarrier(x, y - 1);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        return TileDictionaries.BarrierTileDictionary[lookupIndex];
    }

    int CalculateShoreTile(int x, int y, int[,] level)
    {
        int top = GetTypeForTileAboveShore(x, y + 1),
            left = GetTypeForTileAdjacentToShore(x - 1, y),
            right = GetTypeForTileAdjacentToShore(x + 1, y),
            bottom = GetTypeForTileAdjacentToShore(x, y - 1);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        if (TileDictionaries.ShoreDictionary.ContainsKey(lookupIndex))
        {
            return TileDictionaries.ShoreDictionary[lookupIndex];
        }

        return -1;
    }

    int GetTypeForTileAboveShore(int x, int y)
    {
        if (y >= level.GetLength(0) || level[y, x] == BaseWater)
        {
            return 0;
        }

        bool leftWater = x + 1 >= level.GetLength(1) || level[y, x + 1] == BaseWater;
        bool rightWater = x - 1 < 0 || level[y, x - 1] == BaseWater;

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

    int GetTypeForTileAdjacentToShore(int x, int y)
    {
        return x < 0 || x >= level.GetLength(1) || y < 0 || y >= level.GetLength(0) || level[y, x] == BaseWater ? 0 : 1;
    }

    int GetTypeForBarrier(int x, int y)
    {
        if (x < 0 || y < 0 || x >= level.GetLength(1) || y >= level.GetLength(0))
        {
            return 1;
        }

        int index = level[y, x];
        return IsFloor(index) || IsTree(index) ? 0 : 1;
    }

    int GetTypeForNonTopBarrier(int x, int y)
    {
        if (x < 0 || y < 0 || x >= level.GetLength(1) || y >= level.GetLength(0))
        {
            return 1;
        }

        if (GetTypeForBarrier(x, y) == 0)
        {
            return 0;
        }

        // 0 = floor, 1 = elevated non-wall, 2 = wall
        return GetTypeForBarrier(x, y - 1) == 1 ? 1 : 2;
    }

    int CalculateGrassTile(int x, int y, Dictionary<int, GrassObj[]> grassDictionary)
    {
        int top = GetTypeForGrassTile(x, y, 0, 1),
            left = GetTypeForGrassTile(x, y, -1, 0),
            right = GetTypeForGrassTile(x, y, 1, 0),
            bottom = GetTypeForGrassTile(x, y, 0, -1);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        GrassObj grassObject = grassDictionary[lookupIndex][Random.Range(0, grassDictionary[lookupIndex].Length)];
        if (grassObject.DecorateEdges)
        {
            SetSurroundingGrass(x, y);
        }

        return grassObject.Index;
    }

    int GetTypeForGrassTile(int x, int y, int xOffset, int yOffset)
    {
        int newX = x + xOffset, newY = y + yOffset;
        if (newX < 0 || newY < 0 || newX >= level.GetLength(1) || newY >= level.GetLength(0))
        {
            return GrassIndexType(level[y, x]);
        }

        return GrassIndexType(level[newY, newX]);
    }

    void SetSurroundingGrass(int x, int y)
    {
        if (x + 1 < level.GetLength(1) && !IsDarkGrass(level[y, x + 1]))
        {
            Tile.SetTile(new Int2(x + 1, y), AdditionalGrassLayer1, TilesetIndex, 94, false);
        }

        if (x - 1 > 0 && !IsDarkGrass(level[y, x - 1]))
        {
            Tile.SetTile(new Int2(x - 1, y), AdditionalGrassLayer2, TilesetIndex, 93, false);
        }

        if (y + 1 < level.GetLength(0) && !IsDarkGrass(level[y + 1, x]))
        {
            Tile.SetTile(new Int2(x, y + 1), AdditionalGrassLayer3, TilesetIndex, 92, false);
        }

        if (y - 1 > 0 && !IsDarkGrass(level[y - 1, x]))
        {
            Tile.SetTile(new Int2(x, y - 1), AdditionalGrassLayer4, TilesetIndex, 95, false);
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
                Tile.SetTile(new Int2(col, row), FloorLayer, TilesetIndex,
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
