using UnityEngine;
using SpriteTile;

public class Building : MonoBehaviour {
    public static int TilesetIndex = 3;

    private int[][] BaseTiles;
    private int[][] HighRiseTiles;

    public Building(int[][] baseTiles, int[][] highRiseTiles)
    {
        if (baseTiles.GetLength(1) != highRiseTiles.GetLength(1))
        {
            throw new System.Exception("Base tiles and high rise tiles must have the same number of columns.");
        }

        BaseTiles = baseTiles;
        HighRiseTiles = highRiseTiles;
    }

    // TODO: Set sorting layers.
    public void Construct(int startRow, int startCol)
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
                    tileNum = BaseTiles[BaseTiles.GetLength(0) - row - 1][col];
                } else
                {
                    tileNum = HighRiseTiles[BaseTiles.GetLength(1) - (row - BaseTiles.GetLength(0)) - 1][col];
                }

                Tile.SetTile(new Int2(col, row), 1, TilesetIndex, tileNum, false);
            }
        }
    }
}
