using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class BasicLevelDecorator {
    private static int TilesetIndex = 3;

    // TODO: parameterize
    private static int Ground = 0, Flowers = 1;
    public static int BaseLayer = 0, CliffLayer = 1;

	public void CreateTilemap(int[,] level, Dictionary<int, int> barrierTileDictionary)
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
                    Tile.SetTile(tileLocation, BaseLayer, TilesetIndex, Flowers, false);
                }
                
                if (index > 1)
                {
                    int cliffTile = CalculateBarrierTile(col, row, level, barrierTileDictionary);
                    Tile.SetTile(tileLocation, CliffLayer, TilesetIndex, cliffTile, true);
                }
            }
        }
    }

    int CalculateBarrierTile(int x, int y, int[,] level, Dictionary<int, int> barrierTileDictionary)
    {
        int top = GetType(x, y + 1, level),
            left = GetTypeForNonTop(x - 1, y, level),
            right = GetTypeForNonTop(x + 1, y, level),
            bottom = GetTypeForNonTop(x, y - 1, level);
        int lookupIndex = top * 1000 + left * 100 + right * 10 + bottom;
        return barrierTileDictionary[lookupIndex];
    }

    int GetType(int x, int y, int[,] level)
    {
        if (x < 0 || y < 0 || x >= level.GetLength(1) || y >= level.GetLength(0))
        {
            return 1;
        }

        int index = level[y, x];
        return index == 0 || index == 1 ? 0 : 1;
    }

    int GetTypeForNonTop(int x, int y, int[,] level)
    {
        if (x < 0 || y < 0 || x >= level.GetLength(1) || y >= level.GetLength(0))
        {
            return 1;
        }

        if (GetType(x, y, level) == 0)
        {
            return 0;
        }

        return GetType(x, y - 1, level) == 1 ? 1 : 2;
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
}
