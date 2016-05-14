using SpriteTile;

public class Building {
    public static int TilesetIndex = 3;
    public static int BaseLayerIndex = 1;

    private int[,] BaseTiles;
    private int[,] HighRiseTiles;

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

    public void Render(int startRow, int startCol)
    {
        int numRows = BaseTiles.GetLength(0) + HighRiseTiles.GetLength(0);
        int numCols = BaseTiles.GetLength(1);

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                int tileNum;
                if (row < BaseTiles.GetLength(0))
                {
                    tileNum = BaseTiles[BaseTiles.GetLength(0) - row - 1, col];
                } else
                {
                    tileNum = HighRiseTiles[BaseTiles.GetLength(1) - (row - BaseTiles.GetLength(0)) - 1, col];
                }

                Tile.SetTile(new Int2(col, row), BaseLayerIndex + row, TilesetIndex, tileNum, false);
            }
        }
    }
}
