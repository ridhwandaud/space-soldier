using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelDecorator {
    private static int TilesetIndex = 3;

    private static int LightGrass = 0;
    public static int BaseLayer = 0, CliffLayer = 5;

	public void CreateTilemap(int[,] level)
    {
        SetUpMap(level);
        IdentifyIslands(level);

        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                int index = level[row, col];

                // In internal grid array, first dimension is row and second is col. When placing objects in game, first dimension
                // is x (col) and second is y (row). So they are flipped.
                Int2 tileLocation = new Int2(col, row);

                if (index == 1 || index == 3)
                {
                    Tile.SetTile(tileLocation, BaseLayer, TilesetIndex, CalculateGrassTile(col, row, level, darkGrassDictionary), false);
                }
                
                if (index == 2 || index == 3)
                {
                    int cliffTile = CalculateBarrierTile(col, row, level);
                    Tile.SetTile(tileLocation, CliffLayer, TilesetIndex, cliffTile, true);
                }
            }
        }
    }

    void IdentifyIslands(int[,] level)
    {
        int[,] tracker = new int[level.GetLength(0), level.GetLength(1)];

        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                if ((level[row, col] == 2 || level[row, col] == 3) && tracker[row, col] == 0)
                {
                    IdentifyIsland(row, col, level, tracker);
                }
            }
        }
    }

    void IdentifyIsland(int x, int y, int[,] level, int[,] tracker)
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

            // Extract out logic for identifying an elevated tile.
            if (pos.x - 1 > 0 && level[pos.x - 1, pos.y] > 1 && tracker[pos.x - 1, pos.y] == 0)
            {
                count++;
                queue.Enqueue(new Int2(pos.x - 1, pos.y));
                tracker[pos.x - 1, pos.y] = 1; // have to set this here if you ever want the damn thing to end
            }
            if (pos.x + 1 < level.GetLength(0) && level[pos.x + 1, pos.y] > 1 && tracker[pos.x + 1, pos.y] == 0)
            {
                count++;
                queue.Enqueue(new Int2(pos.x + 1, pos.y));
                tracker[pos.x + 1, pos.y] = 1;
            }
            if (pos.y - 1 > 0 && level[pos.x, pos.y - 1] > 1 && tracker[pos.x, pos.y - 1] == 0)
            {
                count++;
                queue.Enqueue(new Int2(pos.x, pos.y - 1));
                tracker[pos.x, pos.y - 1] = 1;
            }
            if (pos.y + 1 < level.GetLength(1) && level[pos.x, pos.y + 1] > 1 && tracker[pos.x, pos.y + 1] == 0)
            {
                count++;
                queue.Enqueue(new Int2(pos.x, pos.y + 1));
                tracker[pos.x, pos.y + 1] = 1;
            }
              
        }

        if (!touchingEdge && Random.Range(0, 3) < 2)
        {
            foreach (Int2 islandSquare in islandMembers)
            {
                level[islandSquare.x, islandSquare.y] = 5;
                int waterIndex = islandSquare.x + 1 < level.GetLength(0) && !seenThisIteration[islandSquare.x + 1, islandSquare.y] ? 53 : 42;
                Tile.SetTile(new Int2(islandSquare.y, islandSquare.x), CliffLayer, TilesetIndex, waterIndex, true);
            }
        }
    }

    int CalculateBarrierTile(int x, int y, int[,] level)
    {
        int top = GetTypeForBarrier(x, y + 1, level),
            left = GetTypeForNonTopBarrier(x - 1, y, level),
            right = GetTypeForNonTopBarrier(x + 1, y, level),
            bottom = GetTypeForNonTopBarrier(x, y - 1, level);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        return barrierTileDictionary[lookupIndex];
    }

    int GetTypeForBarrier(int x, int y, int[,] level)
    {
        if (x < 0 || y < 0 || x >= level.GetLength(1) || y >= level.GetLength(0))
        {
            return 1;
        }

        int index = level[y, x];
        return index == 0 || index == 1 ? 0 : 1;
    }

    int GetTypeForNonTopBarrier(int x, int y, int[,] level)
    {
        if (x < 0 || y < 0 || x >= level.GetLength(1) || y >= level.GetLength(0))
        {
            return 1;
        }

        if (GetTypeForBarrier(x, y, level) == 0)
        {
            return 0;
        }

        return GetTypeForBarrier(x, y - 1, level) == 1 ? 1 : 2;
    }

    int CalculateGrassTile(int x, int y, int[,] level, Dictionary<int, GrassObj[]> grassDictionary)
    {
        int top = GetTypeForGrassTile(x, y, 0, 1, level),
            left = GetTypeForGrassTile(x, y, -1, 0, level),
            right = GetTypeForGrassTile(x, y, 1, 0, level),
            bottom = GetTypeForGrassTile(x, y, 0, -1, level);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        GrassObj grassObject = grassDictionary[lookupIndex][Random.Range(0, grassDictionary[lookupIndex].Length)];
        if (grassObject.DecorateEdges)
        {
            SetSurroundingGrass(x, y, level);
        }

        return grassObject.Index;
    }

    int GetTypeForGrassTile(int x, int y, int xOffset, int yOffset, int[,] level)
    {
        int newX = x + xOffset, newY = y + yOffset;
        if (newX < 0 || newY < 0 || newX >= level.GetLength(1) || newY >= level.GetLength(0))
        {
            return GrassIndexType(level[y, x]);
        }

        return GrassIndexType(level[newY, newX]);
    }

    void SetSurroundingGrass(int x, int y, int[,] level)
    {
        if (x + 1 < level.GetLength(1) && !IsDarkGrass(level[y, x + 1]))
        {
            Tile.SetTile(new Int2(x + 1, y), 1, TilesetIndex, 94, false);
        }

        if (x - 1 > 0 && !IsDarkGrass(level[y, x - 1]))
        {
            Tile.SetTile(new Int2(x - 1, y), 2, TilesetIndex, 93, false);
        }

        if (y + 1 < level.GetLength(0) && !IsDarkGrass(level[y + 1, x]))
        {
            Tile.SetTile(new Int2(x, y + 1), 3, TilesetIndex, 92, false);
        }

        if (y - 1 > 0 && !IsDarkGrass(level[y - 1, x]))
        {
            Tile.SetTile(new Int2(x, y - 1), 4, TilesetIndex, 95, false);
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

    void SetUpMap(int[,] level)
    {
        Int2 mapDimensions = new Int2(level.GetLength(1), level.GetLength(0));

        // create level
        Tile.NewLevel(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);

        // set collider layer so that walls can be detected by raycasting
        Tile.SetColliderLayer(GameSettings.WallLayerNumber);

        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                Tile.SetTile(new Int2(col, row), BaseLayer, TilesetIndex, LightGrass, false);
            }
        }
    }

    private static Dictionary<int, int> barrierTileDictionary = new Dictionary<int, int>
    {
        {2, 26}, // SingleElevated
        {1012, 31}, // LowerLeftElevated
        {1112, 0}, // LowerElevated
        {1102, 34}, // LowerRightElevated
        {12, 22}, // LeftThinElevated
        {112, 23}, // HorizontalThinElevated
        {102, 24}, // RightThinElevated
        {1000, 33}, // SingleWall
        {1010, 36}, // LeftWall
        {1110, 37}, // MiddleWall
        {1100, 38}, // RightWall
        {1, 26}, // TopThinElevated
        {11, 22}, // TopLeftElevated
        {1101, 32}, // RightEdgeElevated
        {1011, 31}, // LeftEdgeElevated
        {1111, 0}, // Elevated
        {1001, 25}, // VerticalThinElevated - there are now different types for when it joins with stuff (35 vs. 37) - account for this later.
        {1002, 25}, // BottomThinElevated
        {111, 23}, // TopEdgeElevated
        {101, 24}, // TopRightElevated
        {1212, 51}, // LowerLeftElevatedNextToWall
        {1122, 50}, // RightEdgeElevatedNextToWall
        {1222, 25}, // BottomThinElevated
        {1221, 25}, // VerticalThinElevated
        {1211, 51}, // LowerLeftElevatedNextToWall
        {1121, 50}, // RightEdgeElevatedNextToWall
        {1020, 36}, // LeftWall
        {1021, 25}, // VerticalThinElevated
        {1022, 25}, // BottomThinElevated
        {1200, 38}, // RightWall
        {1201, 25}, // VerticalThinElevated
        {1202, 25}, // BottomThinElevated
        {1120, 37}, // MiddleWall
        {1210, 37}, // MiddleWall
        {121, 24}, // TopRightElevated
        {211, 22}, // TopLeftElevated
        {221, 25}, // TopThinElevated
        {222, 26}, // SingleElevated
        {202, 26}, // SingleElevated
        {212, 22}, // LeftThinElevated
        {201, 26}, // TopThinElevated
        {1220, 37}, // MiddleWall
        {21, 25}, // TopThinElevated
        {22, 26}, // SingleElevated
        {122, 24}, // RightThinElevated

        // TODO: These next ones are lone blocks. Make them into rocks or other obstacles.
        {0, 33},
        {10, 33},
        {100, 33},
        {110, 33},
        {120, 33},
        {200, 33},
        {20, 33},
        {210, 33},
        {220, 33}
    };

    private static Dictionary<int, GrassObj[]> darkGrassDictionary = new Dictionary<int, GrassObj[]>
    {
        {0000, new GrassObj[] { new GrassObj(1, false) }},
        {0001, new GrassObj[] { new GrassObj(96, false) }},
        {0010, new GrassObj[] { new GrassObj(97, false) }},
        {0011, new GrassObj[] { new GrassObj (3, false) }}, 
        {0100, new GrassObj[] { new GrassObj(99, false) }},
        {0101, new GrassObj[] { new GrassObj (5, false) }},
        {0110, new GrassObj[] { new GrassObj(1, true) }},
        {0111, new GrassObj[] { new GrassObj (4, false), new GrassObj(1, true) }},
        {1000, new GrassObj[] { new GrassObj(98, false) }},
        {1001, new GrassObj[] { new GrassObj(1, true) }},
        {1010, new GrassObj[] { new GrassObj(15, false) }},
        {1011, new GrassObj[] { new GrassObj (11, false), new GrassObj(1, true) }},
        {1100, new GrassObj[] { new GrassObj(16, false) }},
        {1101, new GrassObj[] { new GrassObj (10, false), new GrassObj(1, true) }},
        {1110, new GrassObj[] { new GrassObj (2, false), new GrassObj(1, true) }},
        {1111, new GrassObj[] { new GrassObj(1, false) }},
    };

    struct GrassObj
    {
        public int Index;
        public bool DecorateEdges;

        public GrassObj(int index, bool decorateEdges)
        {
            Index = index;
            DecorateEdges = decorateEdges;
        }
    }

}
