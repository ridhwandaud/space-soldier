using UnityEngine;
using System.Collections.Generic;

public class CityDecorator {
    private static int RoadThickness = 2;

    // Minimum size of a building, as defined in the list.
    private static int MinStructureWidth = 3;
    private static int MinStructureHeight = 2;

    // If the divided dimension is greater than the corresponding max, a division WILL happen.
    private static int MaxBlockWidth = 6;
    private static int MaxBlockHeight = 6;

	public void GenerateBuildings(List<Rect> cityBlocks, int[,] tilemap)
    {
        foreach (Rect cityBlock in cityBlocks)
        {
            int numAttempts = 0;
            Queue<Rect> rectQueue = new Queue<Rect>();
            rectQueue.Enqueue(new Rect(cityBlock.xMin + RoadThickness, cityBlock.yMin, cityBlock.size.x - RoadThickness,
                cityBlock.size.y - RoadThickness));
            while (rectQueue.Count > 0)
            {
                numAttempts++;
                Rect curr = rectQueue.Dequeue();
                int width = (int)curr.size.x, height = (int)curr.size.y;

                bool canDivide = width >= MinStructureWidth * 2 || height >= MinStructureHeight * 2;
                bool vert = width > height;

                float dividedDimensionLength = vert ? width : height;
                float maxDimension = vert ? MaxBlockWidth : MaxBlockHeight;

                if (canDivide && (dividedDimensionLength > maxDimension || Random.Range(0, 4) < 2))
                {
                    // Divide
                    float padding = vert ? MinStructureWidth : MinStructureHeight;
                    float divisionOffset = Random.Range((int)padding, (int)(dividedDimensionLength - padding));
                    rectQueue.Enqueue(new Rect(
                        curr.xMin,
                        curr.yMin,
                        vert ? divisionOffset : curr.width,
                        vert ? curr.height : divisionOffset));
                    rectQueue.Enqueue(new Rect(
                        vert ? curr.xMin + divisionOffset : curr.xMin ,
                        vert ? curr.yMin : curr.yMin + divisionOffset,
                        vert ? width - divisionOffset : width,
                        vert ? height : height - divisionOffset));
                } else
                {
                    // Place building. Tiles from xMin, yMin (inclusive) to xMax, yMax (exclusive) can be potentially filled.
                    CityGenerator.RenderRect(curr, 2);
                }
            }
        }
    }
}
