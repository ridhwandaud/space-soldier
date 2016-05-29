using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class CityGridCreator {
    private static int Padding = 4;
    private static int NormalizationOffsetX;
    private static int NormalizationOffsetY;
    public static int GridArrayRoadIndex = 1;
    private static int RoadTileIndex = 1;
    public static int DefaultWalkableIndex = 0;
    private static int PerimeterArrayIndex = 3;
    private static int PerimeterTileIndex = 2;

    private static int TileSetIndex = 0;

    public static int RoadThickness = 3;

	public int[,] GenerateGrid(List<Road> perimeterLines, List<Rect> rectangles)
    {
        int[,] result = createEmptyArray(perimeterLines);

        Int2 mapDimensions = new Int2(result.GetLength(1), result.GetLength(0));
        Tile.NewLevel(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);

        for (int i = 0; i < CityAlgorithm.MaxRectangleHeight; i++)
        {
            Tile.AddLayer(mapDimensions, 0, GameSettings.TileSize, 0, LayerLock.None);

            if (i >= Building.BaseBuildingIndex)
            {
                // Treat all building layers as walls.
                Tile.SetColliderLayer(GameSettings.WallLayerNumber, i);
            }
        }

        for (int row = 0; row < result.GetLength(0); row++)
        {
            for (int col = 0; col < result.GetLength(1); col++)
            {
                result[row, col] = 0;
                Tile.SetTile(new Int2(col, row), TileSetIndex, 0, false);
            }
        }

        PlacePerimeterTiles(result, perimeterLines);
        PlaceRoads(result, rectangles);

        return result;
    }

    void PlacePerimeterTiles(int[,] grid, List<Road> perimeterLines)
    {
        foreach (Road line in perimeterLines)
        {
            DrawGridLine(grid, (int)line.Endpoint1.x, (int)line.Endpoint1.y, (int)line.Endpoint2.x, (int)line.Endpoint2.y,
                PerimeterTileIndex, PerimeterArrayIndex, Building.BaseBuildingIndex, true);
        }
    }

    void PlaceRoads(int[,] grid, List<Rect> rects)
    {
        foreach (Rect r in rects)
        {
            for (int i = 0; i < RoadThickness; i++)
            {
                DrawGridLine(grid, (int)r.xMin + i, (int)r.yMin, (int)r.xMin + i, (int)r.yMax, RoadTileIndex,
                    GridArrayRoadIndex, 0, false);
                DrawGridLine(grid, (int)r.xMax + i, (int)r.yMin - RoadThickness + 1, (int)r.xMax + i, (int)r.yMax,
                    RoadTileIndex, GridArrayRoadIndex, 0, false);
                DrawGridLine(grid, (int)r.xMin, (int)r.yMin - i, (int)r.xMax, (int)r.yMin - i, RoadTileIndex,
                    GridArrayRoadIndex, 0, false);
                DrawGridLine(grid, (int)r.xMin, (int)r.yMax - i, (int)r.xMax, (int)r.yMax - i, RoadTileIndex,
                    GridArrayRoadIndex, 0, false);
            }
        }
    }

    void DrawGridLine(int[,] grid, int x1, int y1, int x2, int y2, int tileIndex, int arrayIndex, int layer,
        bool isCollider)
    {
        bool vert = x1 == x2;
        if (vert)
        {
            int minY = Mathf.Min(y1, y2);
            int maxY = Mathf.Max(y1, y2);
            for (int i = minY; i <= maxY; i++)
            {
                int y = NormalizeY(i);
                int x = NormalizeX(x1);
                grid[y, x] = arrayIndex;
                Tile.SetTile(new Int2(x, y), layer, TileSetIndex, tileIndex, isCollider);
            }
        }
        else
        {
            int minX = Mathf.Min(x1, x2);
            int maxX = Mathf.Max(x1, x2);
            for (int i = minX; i <= maxX; i++)
            {
                int x = NormalizeX(i);
                int y = NormalizeY(y1);
                grid[y, x] = arrayIndex;
                Tile.SetTile(new Int2(x, y), layer, TileSetIndex, tileIndex, isCollider);
            }
        }
    }

    int[,] createEmptyArray(List<Road> perimeterLines)
    {
        int minX = 1000, maxX = -1, minY = 1000, maxY = -1;

        foreach (Road line in perimeterLines)
        {
            if (line.Endpoint1.x < minX)
            {
                minX = (int)line.Endpoint1.x;
            }
            if (line.Endpoint2.x < minX)
            {
                minX = (int)line.Endpoint2.x;
            }
            if (line.Endpoint1.x > maxX)
            {
                maxX = (int)line.Endpoint1.x;
            }
            if (line.Endpoint2.x > maxX)
            {
                maxX = (int)line.Endpoint2.x;
            }
            if (line.Endpoint1.y < minY)
            {
                minY = (int)line.Endpoint1.y;
            }
            if (line.Endpoint2.y < minY)
            {
                minY = (int)line.Endpoint2.y;
            }
            if (line.Endpoint1.y > maxY)
            {
                maxY = (int)line.Endpoint1.y;
            }
            if (line.Endpoint2.y > maxY)
            {
                maxY = (int)line.Endpoint2.y;
            }
        }

        NormalizationOffsetX = minX - Padding;
        NormalizationOffsetY = minY - Padding;

        return new int[maxY - minY + 2 * Padding + 1, maxX - minX + 2 * Padding + 1];
    }

    public static int NormalizeX(int val)
    {
        return val - NormalizationOffsetX;
    }

    public static int NormalizeY(int val)
    {
        return val - NormalizationOffsetY;
    }
}
