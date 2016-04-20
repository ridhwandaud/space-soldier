using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelDecorator {
    private static int TilesetIndex = 3;

    // TODO: parameterize
    private static int Ground = 0, Flowers = 1;
    public static int BaseLayer = 0, CliffLayer = 1;

	public void CreateTilemap(int[,] level)
    {
        SetUpMap(level);

        for (int row = 0; row < level.GetLength(0); row++)
        {
            for (int col = 0; col < level.GetLength(1); col++)
            {
                int index = level[row, col];

                // In internal grid array, first dimension is row and second is col. When placing objects in game, first dimension
                // is x (col) and second is y (row). So they are flipped.
                Int2 tileLocation = new Int2(col, row);

                if (index == 0 || index == 2)
                {
                    Tile.SetTile(tileLocation, BaseLayer, TilesetIndex, Ground, false);
                } else if (index == 1 || index == 3)
                {
                    Tile.SetTile(tileLocation, BaseLayer, TilesetIndex, CalculateGrassTile(col, row, level, darkGrassDictionary), false);
                }
                
                if (index > 1)
                {
                    int cliffTile = CalculateBarrierTile(col, row, level);
                    Tile.SetTile(tileLocation, CliffLayer, TilesetIndex, cliffTile, true);
                }
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

    int CalculateGrassTile(int x, int y, int[,] level, Dictionary<int, int> grassDictionary)
    {
        int top = GetTypeForGrassTile(x, y, 0, 1, level),
            left = GetTypeForGrassTile(x, y, -1, 0, level),
            right = GetTypeForGrassTile(x, y, 1, 0, level),
            bottom = GetTypeForGrassTile(x, y, 0, -1, level);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        SetSurroundingGrass(x, y, level);
        return grassDictionary[lookupIndex];
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
        if (x + 1 < level.GetLength(1) && level[y, x + 1] % 2 == 0)
        {
            Tile.SetTile(new Int2(x + 1, y), BaseLayer, TilesetIndex, 94, false);
        }

        if (x - 1 > 0 && level[y, x - 1] % 2 == 0)
        {
            Tile.SetTile(new Int2(x - 1, y), BaseLayer, TilesetIndex, 93, false);
        }

        if (y + 1 < level.GetLength(0) && level[y + 1, x] % 2 == 0)
        {
            Tile.SetTile(new Int2(x, y + 1), BaseLayer, TilesetIndex, 92, false);
        }

        if (y - 1 > 0 && level[y - 1, x] % 2 == 0)
        {
            Tile.SetTile(new Int2(x, y - 1), BaseLayer, TilesetIndex, 95, false);
        }
    }

    int GrassIndexType(int index)
    {
        return index % 2 == 0 ? 0 : 1;
    }

    void SetUpMap(int[,] level)
    {
        Int2 mapDimensions = new Int2(level.GetLength(1), level.GetLength(0));

        // create level
        Tile.NewLevel(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);
        Tile.AddLayer(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);

        // set collider layer so that walls can be detected by raycasting
        Tile.SetColliderLayer(GameSettings.WallLayerNumber);
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
        {201, 25}, // TopThinElevated
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

    private static Dictionary<int, int> darkGrassDictionary = new Dictionary<int, int>
    {
        {0000, 1}, // eventually change to circle
        {0001, 1}, // was 92
        {0010, 1}, // was 93
        {0011, 3}, 
        {0100, 1}, // was 94
        {0101, 5},
        {0110, 1}, // eventually change surrounding blox
        {0111, 4},
        {1000, 1}, // was 95
        {1001, 1}, // eventually change surrounding blox
        {1010, 15},
        {1011, 11},
        {1100, 16},
        {1101, 10},
        {1110, 2},
        {1111, 1},
    };

    private static Dictionary<int, int> lightGrassDictionary = new Dictionary<int, int>
    {
        {0000, 0},
        {0001, 0},
        {0010, 0},
        {0011, 0},
        {0100, 0},
        {0101, 0},
        {0110, 0},
        {0111, 0}, // was 4
        {1000, 0},
        {1001, 0}, // eventually change surrounding blox
        {1010, 0},
        {1011, 0}, // was 11
        {1100, 0},
        {1101, 0}, // was 10
        {1110, 0}, // was 2
        {1111, 0}, // eventually change to circle
    };
}
