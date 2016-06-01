using SpriteTile;
using UnityEngine;

public class Building {
    public static int TilesetIndex = 3;
    public static int BaseBuildingIndex = 1;
    public static int HighRiseBaseBuildingIndex = 12;
    private static int GridArrayBuildingIndex = 2;

    public int[,] BaseTiles { get; set; }
    public int[,] HighRiseTiles { get; set; }

    public int NumBaseRows
    {
        get
        {
            return BaseTiles.GetLength(0);
        }
    }

    public int NumRows
    {
        get
        {
            return BaseTiles.GetLength(0) + HighRiseTiles.GetLength(0);
        }
    }

    public int NumCols
    {
        get
        {
            return BaseTiles.GetLength(1);
        }
    }

    public Building (int[,] highRiseTiles, int[,] baseTiles)
    {
        if (baseTiles.GetLength(1) != highRiseTiles.GetLength(1))
        {
            throw new System.Exception("Base tiles and high rise tiles must have the same number of columns.");
        }

        BaseTiles = baseTiles;
        HighRiseTiles = highRiseTiles;
    }

    public bool Render (int startRow, int startCol, int[,] grid)
    {
        int numRows = BaseTiles.GetLength(0) + HighRiseTiles.GetLength(0);
        int numCols = BaseTiles.GetLength(1);

        for (int rowOffset = 0; rowOffset < NumBaseRows; rowOffset++)
        {
            for (int colOffset = 0; colOffset < numCols; colOffset++)
            {
                // SpriteTile returns a -1 if there is no tile at the given location.
                if (Tile.GetTile(new Int2(startCol + colOffset, startRow + rowOffset), BaseBuildingIndex).tile != -1)
                {
                    return false;
                }
            }
        }

        for (int rowOffset = 0; rowOffset < numRows; rowOffset++)
        {
            for (int colOffset = 0; colOffset < numCols; colOffset++)
            {
                int tileNum;
                bool setCollider = false;
                if (rowOffset < NumBaseRows)
                {
                    tileNum = BaseTiles[BaseTiles.GetLength(0) - rowOffset - 1, colOffset];
                    grid[startRow + rowOffset, startCol + colOffset] = GridArrayBuildingIndex;
                    setCollider = true;
                } else
                {
                    tileNum = HighRiseTiles[HighRiseTiles.GetLength(0) - (rowOffset - BaseTiles.GetLength(0)) - 1, colOffset];
                }

                int layer = rowOffset < NumBaseRows ? BaseBuildingIndex : HighRiseBaseBuildingIndex + rowOffset;
                Tile.SetTile(new Int2(startCol + colOffset, startRow + rowOffset), layer, TilesetIndex, tileNum, setCollider);
            }
        }

        return true;
    }
}
