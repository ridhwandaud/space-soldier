using UnityEngine;
using System.Collections.Generic;

public class CityDecorator {
    private static int RoadThickness = 2;

    // Minimum size of a building, as defined in the list.
    private static int MinStructureWidth = 4;
    private static int MinStructureHeight = 4;

    // If the divided dimension is greater than the corresponding max, a division WILL happen.
    private static int MaxBlockWidth = 6;
    private static int MaxBlockHeight = 6;

    private Dictionary<int, Dictionary<int, List<Building>>> BuildingDictionary;

    public void GenerateBuildings (List<Rect> cityBlocks, int[,] tilemap)
    {
        ConstructBuildingDictionary();

        foreach (Rect cityBlock in cityBlocks)
        {
            int numAttempts = 0;
            Queue<Rect> rectQueue = new Queue<Rect>();
            rectQueue.Enqueue(new Rect(cityBlock.xMin + RoadThickness, cityBlock.yMin + 1, cityBlock.size.x - RoadThickness,
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
                    //CityGenerator.RenderRect(curr, 2);
                    SelectAndPlaceBuilding(curr);
                }
            }
        }
    }

    void SelectAndPlaceBuilding(Rect rect)
    {
        List<Building> potentialBuildings = new List<Building>();
        for (int row = (int)rect.height - 3; row <= rect.height; row++)
        {
            for (int col = (int)rect.width - 3; col <= rect.width; col++)
            {
                if (BuildingDictionary.ContainsKey(row) && BuildingDictionary[row].ContainsKey(col))
                {
                    potentialBuildings.AddRange(BuildingDictionary[row][col]);
                }
            }
        }

        int rand = Random.Range(0, potentialBuildings.Count);

        if (potentialBuildings.Count == 0)
        {
            Debug.Log("Unpopulatable rect with " + rect.height + " rows and " + rect.width + " columns.");
            return;
        }

        Building selectedBuilding = potentialBuildings[rand];

        int rowOffset = Random.Range(0, (int)rect.height - selectedBuilding.NumRows + 1);
        int colOffset = Random.Range(0, (int)rect.width - selectedBuilding.NumCols + 1);

        selectedBuilding.Render(CityGridCreator.NormalizeY((int)rect.y + 0), CityGridCreator.NormalizeX((int)rect.x + 0));
    }

    void ConstructBuildingDictionary ()
    {
        Dictionary<int, Dictionary<int, List<Building>>> result = new Dictionary<int, Dictionary<int, List<Building>>>();

        foreach (Building b in Buildings)
        {
            if (!result.ContainsKey(b.NumRows))
            {
                result[b.NumRows] = new Dictionary<int, List<Building>>();
            }

            Dictionary<int, List<Building>> rowDict = result[b.NumRows];

            if (!rowDict.ContainsKey(b.NumCols))
            {
                rowDict[b.NumCols] = new List<Building>();
            }

            List<Building> buildings = rowDict[b.NumCols];

            buildings.Add(b);
        }

        BuildingDictionary = result;
    }

    private static List<Building> Buildings = new List<Building> {
            new Building(
                new int[,] {
                    { 162, 163, 164, 165 }
                },
                new int[,] {
                    { 181, 182, 183, 14 },
                    { 200, 201, 202, 203 },
                    { 220, 221, 222, 223 }
                }),
            new Building(
                new int[,] {
                    { 70, 71, 72, 73 }
                },
                new int[,] {
                    { 86, 87, 88, 89 },
                    { 98, 99, 100, 101 },
                    { 110, 111, 112, 113 }
                }),
            new Building(
                new int[,] {
                    { 171, 172 }
                },
                new int[,] {
                    { 190, 191 }
                }),
    };
}
