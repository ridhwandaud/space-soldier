using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CityDecorator {
    private static int SizeCushionAmount = 4;
    private static int PerimeterBuildingOffset = 5;
    private static int AllowedBuildingSizeDiff = 4;
    private static int AllowedPerimeterBuildingSizeDiff = 2;
    private static int MinStructureWidth;
    private static int MinStructureHeight;
    private static int MaxStructureWidth;
    private static int MaxStructureHeight;

    private Dictionary<int, Dictionary<int, List<Building>>> BuildingDictionary;

    public void GenerateBuildings (List<Rect> cityBlocks, int[,] tilemap)
    {
        ConstructBuildingDictionary();
        int maxBlockWidth = MaxStructureWidth + SizeCushionAmount, maxBlockHeight = MaxStructureHeight + SizeCushionAmount;

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
                bool vert = (canDivideVertically && canDivideHorizontally) ? height - maxBlockHeight < width - maxBlockWidth : canDivideVertically;

                float dividedDimensionLength = vert ? width : height;
                float maxDimension = vert ? maxBlockWidth : maxBlockHeight;

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
                    Building selectedBuilding = SelectBuilding((int)curr.width, (int)curr.height, (int)curr.yMin, AllowedBuildingSizeDiff, ceiling);
                    if (selectedBuilding !=  null)
                    {
                        int rowOffset = Random.Range(0, (int)curr.height - selectedBuilding.NumBaseRows + 1);
                        int colOffset = Random.Range(0, (int)curr.width - selectedBuilding.NumCols + 1);

                        selectedBuilding.Render(CityGridCreator.NormalizeY((int)curr.y + 0), CityGridCreator.NormalizeX((int)curr.x + colOffset), tilemap);
                    }
                }
            }
        }
    }

    Building SelectBuilding (int width, int height, int yMin, int buildingVariationAmount, int ceiling = 100)
    {
        List<Building> potentialBuildings = new List<Building>();
        for (int row = height - buildingVariationAmount; row <= height; row++)
        {
            for (int col = width - buildingVariationAmount; col <= width; col++)
            {
                if (BuildingDictionary.ContainsKey(row) && BuildingDictionary[row].ContainsKey(col))
                {
                    potentialBuildings.AddRange(BuildingDictionary[row][col].Where(
                        building => building.NumRows <= ceiling - yMin).ToList());
                }
            }
        }

        int rand = Random.Range(0, potentialBuildings.Count);

        if (potentialBuildings.Count == 0)
        {
            //Debug.Log("Unpopulatable rect with " + rect.height + " rows and " + rect.width + " columns.");
            return null;
        }

        return potentialBuildings[rand];
    }

    public void DecoratePerimeters(List<Road> perimeterLines, int[,] tilemap)
    {
        foreach (Road r in perimeterLines)
        {
            int currX = (int)Mathf.Min(r.Endpoint1.x, r.Endpoint2.x) + (r.Side == Side.Left ?
                2 : r.Side == Side.Right ? -PerimeterBuildingOffset : 0);
            int currY = (int)Mathf.Min(r.Endpoint1.y, r.Endpoint2.y) + (r.Side == Side.Bottom ?
                1 : r.Side == Side.Top ? -PerimeterBuildingOffset : 0);
            int maxX = (int)Mathf.Max(r.Endpoint1.x, r.Endpoint2.x);
            int maxY = (int)Mathf.Max(r.Endpoint1.y, r.Endpoint2.y);

            bool vert = r.Endpoint1.x == r.Endpoint2.x;
            bool canContinue = true;
            
            while (canContinue)
            {
                int maxWidth = vert ? PerimeterBuildingOffset : Mathf.Min(MaxStructureWidth, maxX - currX);
                int maxHeight = vert ? Mathf.Min(MaxStructureHeight, maxY - currY) : PerimeterBuildingOffset;

                Building building = SelectBuilding(maxWidth, maxHeight, currY, AllowedPerimeterBuildingSizeDiff);
                
                if (building != null)
                {
                    building.Render(CityGridCreator.NormalizeY(currY), CityGridCreator.NormalizeX(currX), tilemap);

                    currX += vert ? 0 : building.NumCols;
                    currY += vert ? building.NumBaseRows : 0;

                    canContinue = vert ? currY < maxY : currX < maxX;
                } else
                {
                    canContinue = false;
                }
            }
        }
    }

    void ConstructBuildingDictionary ()
    {
        Dictionary<int, Dictionary<int, List<Building>>> result = new Dictionary<int, Dictionary<int, List<Building>>>();
        MinStructureWidth = Buildings[0].NumCols;
        MinStructureHeight = Buildings[0].NumBaseRows;
        int maxWidth = Buildings[0].NumCols;
        int maxHeight = Buildings[0].NumBaseRows;

        foreach (Building b in Buildings)
        {
            MinStructureWidth = Mathf.Min(MinStructureWidth, b.NumCols);
            MinStructureHeight = Mathf.Min(MinStructureHeight, b.NumBaseRows);
            maxWidth = Mathf.Max(maxWidth, b.NumCols);
            maxHeight = Mathf.Max(maxHeight, b.NumBaseRows);

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

        MaxStructureWidth = maxWidth;
        MaxStructureHeight = maxHeight;

        BuildingDictionary = result;
    }

    public static List<Building> Buildings = new List<Building> {
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
