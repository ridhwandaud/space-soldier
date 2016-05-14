using UnityEngine;
using System.Collections.Generic;

public class CityDecorator {
    private static int RoadThickness = 2;

    // Minimum size of a building, as defined in the list.
    private static int MinStructureWidth = 3;
    private static int MinStructureHeight = 3;

    // If the divided dimension is greater than the corresponding max, a division WILL happen.
    private static int MaxBlockWidth = 6;
    private static int MaxBlockHeight = 6;

    private List<List<List<Building>>> BuildingList;

    public void GenerateBuildings (List<Rect> cityBlocks, int[,] tilemap)
    {
        ConstructBuildingArray();
        //for (int x = 0; x < buildingList.Count; x++) {
        //    List<List<Building>> row = buildingList[x];
        //    for(int y = 0; y < row.Count; y++)
        //    {
        //        List<Building> col = row[y];
        //        if(col.Count > 0)
        //        {
        //            Debug.Log("Found " + col.Count + " at row " + x + " col " + y + " .");
        //        }
        //    }
        //}

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
                        vert ? curr.xMin + divisionOffset : curr.xMin,
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

    void SelectAndPlaceBuilding(Rect rect)
    {
        List<Building> potentialBuildings = new List<Building>();
        for (int row = (int)rect.height - 2; row < rect.height; row++)
        {
            for (int col = (int)rect.width - 2; col < rect.width; col++)
            {
                potentialBuildings.AddRange(BuildingList[row][col]);
            }
        }

        Building selectedBuilding = potentialBuildings[Random.Range(0, potentialBuildings.Count)];

        int rowOffset = Random.Range(0, (int)rect.height - selectedBuilding.NumRows + 1);
        int colOffset = Random.Range(0, (int)rect.width - selectedBuilding.NumCols + 1);

        selectedBuilding.Render((int)rect.y + rowOffset, (int)rect.x + colOffset);
    }

    void ConstructBuildingArray ()
    {
        List<List<List<Building>>> result = new List<List<List<Building>>>();

        foreach (Building b in Buildings)
        {
            while (result.Count <= b.NumRows)
            {
                result.Add(new List<List<Building>>());
            }

            List<List<Building>> row = result[b.NumRows];

            while (row.Count <= b.NumCols)
            {
                row.Add(new List<Building>());
            }

            row[b.NumCols].Add(b);
        }

        BuildingList = result;
    }

    private static List<Building> Buildings = new List<Building> {
            new Building(
                new int[,] { 
                    { 3, 3, 3 },
                    { 2, 2, 2 },
                    { 1, 1, 1 }
                },
                new int[,] {
                    { 0, 0, 0 }
                }),
            new Building(
                new int[,] {
                    { 2, 2, 2, 2 },
                    { 1, 1, 1, 1 }
                },
                new int[,] {
                    { 0, 0, 0, 0 }
                }),
    };
}
