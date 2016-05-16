using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CityDecorator {
    // Minimum size of a building, as defined in the list.
    private static int MinStructureWidth = 4;
    private static int MinStructureHeight = 1;

    // If the divided dimension is greater than the corresponding max, a division WILL happen.
    private static int MaxBlockWidth = 6;
    private static int MaxBlockHeight = 6;

    private Dictionary<int, Dictionary<int, List<Building>>> BuildingDictionary;

    public void GenerateBuildings (List<Rect> cityBlocks, int[,] tilemap)
    {
        ConstructBuildingDictionary();

        foreach (Rect cityBlock in cityBlocks)
        {
            Queue<Rect> rectQueue = new Queue<Rect>();
            int ceiling = (int)cityBlock.yMax - CityGridCreator.RoadThickness + 1;
            rectQueue.Enqueue(new Rect(cityBlock.xMin + CityGridCreator.RoadThickness, cityBlock.yMin + 1,
                cityBlock.size.x - CityGridCreator.RoadThickness, cityBlock.size.y - CityGridCreator.RoadThickness));

            while (rectQueue.Count > 0)
            {
                Rect curr = rectQueue.Dequeue();
                int width = (int)curr.size.x, height = (int)curr.size.y;

                bool canDivideVertically = width >= MinStructureWidth * 2;
                bool canDivideHorizontally = height >= MinStructureHeight * 2;
                bool vert = (canDivideVertically && canDivideHorizontally) ? height - MaxBlockHeight < width - MaxBlockWidth : canDivideVertically;

                float dividedDimensionLength = vert ? width : height;
                float maxDimension = vert ? MaxBlockWidth : MaxBlockHeight;

                if ((canDivideHorizontally || canDivideVertically) && (dividedDimensionLength > maxDimension || Random.Range(0, 5) < 1))
                {
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
                    SelectAndPlaceBuilding(curr, ceiling);
                }
            }
        }
    }

    void SelectAndPlaceBuilding(Rect rect, int ceiling)
    {
        List<Building> potentialBuildings = new List<Building>();
        for (int row = (int)rect.height - 4; row <= rect.height; row++)
        {
            for (int col = (int)rect.width - 4; col <= rect.width; col++)
            {
                if (BuildingDictionary.ContainsKey(row) && BuildingDictionary[row].ContainsKey(col))
                {
                    potentialBuildings.AddRange(BuildingDictionary[row][col].Where(
                        building => building.NumRows <= ceiling - rect.yMin).ToList());
                }
            }
        }

        int rand = Random.Range(0, potentialBuildings.Count);

        if (potentialBuildings.Count == 0)
        {
            //Debug.Log("Unpopulatable rect with " + rect.height + " rows and " + rect.width + " columns.");
            return;
        }

        Building selectedBuilding = potentialBuildings[rand];

        int rowOffset = Random.Range(0, (int)rect.height - selectedBuilding.NumBaseRows + 1);
        int colOffset = Random.Range(0, (int)rect.width - selectedBuilding.NumCols + 1);

        selectedBuilding.Render(CityGridCreator.NormalizeY((int)rect.y + 0), CityGridCreator.NormalizeX((int)rect.x + colOffset));
    }

    void ConstructBuildingDictionary ()
    {
        Dictionary<int, Dictionary<int, List<Building>>> result = new Dictionary<int, Dictionary<int, List<Building>>>();

        foreach (Building b in Buildings)
        {
            if (!result.ContainsKey(b.NumBaseRows))
            {
                result[b.NumBaseRows] = new Dictionary<int, List<Building>>();
            }

            Dictionary<int, List<Building>> rowDict = result[b.NumBaseRows];

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
                    { 181, 182, 183, 184 },
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
