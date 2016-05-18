using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SpriteTile;

public class CityDecorator {
    private static int SizeCushionAmount = 4;
    private static int PerimeterBuildingOffset = 5;

    private Dictionary<int, Dictionary<int, List<Building>>> BuildingDictionary;

    public void GenerateBuildings (List<Rect> cityBlocks, int[,] tilemap)
    {
        int minStructureWidth, minStructureHeight, maxBlockWidth, maxBlockHeight;
        ConstructBuildingDictionary(out minStructureWidth, out minStructureHeight, out maxBlockWidth, out maxBlockHeight);

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

                bool canDivideVertically = width >= minStructureWidth * 2;
                bool canDivideHorizontally = height >= minStructureHeight * 2;
                bool vert = (canDivideVertically && canDivideHorizontally) ? height - maxBlockHeight < width - maxBlockWidth : canDivideVertically;

                float dividedDimensionLength = vert ? width : height;
                float maxDimension = vert ? maxBlockWidth : maxBlockHeight;

                if ((canDivideHorizontally || canDivideVertically) && (dividedDimensionLength > maxDimension || Random.Range(0, 5) < 1))
                {
                    float padding = vert ? minStructureWidth : minStructureHeight;
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
                    SelectAndPlaceBuilding(curr, ceiling);
                }
            }
        }
    }

    void SelectAndPlaceBuilding(Rect rect, int ceiling)
    {
        List<Building> potentialBuildings = new List<Building>();
        for (int row = (int)rect.height - SizeCushionAmount; row <= rect.height; row++)
        {
            for (int col = (int)rect.width - SizeCushionAmount; col <= rect.width; col++)
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

    public void DecoratePerimeters(List<Road> perimeterLines)
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

                Building temp = Buildings[1];
                temp.Render(CityGridCreator.NormalizeY(currY), CityGridCreator.NormalizeX(currX));
                //Tile.SetTile(new Int2(CityGridCreator.NormalizeX(currX), CityGridCreator.NormalizeY(currY)), 0, 2, false);

                currX += vert ? 0 : temp.NumCols;
                currY += vert ? temp.NumBaseRows : 0;

                canContinue = vert ? currY < maxY : currX < maxX;
            }
        }
    }

    void ConstructBuildingDictionary (out int minStructureWidth, out int minStructureHeight, out int maxBlockWidth, out int maxBlockHeight)
    {
        Dictionary<int, Dictionary<int, List<Building>>> result = new Dictionary<int, Dictionary<int, List<Building>>>();
        minStructureWidth = Buildings[0].NumCols;
        minStructureHeight = Buildings[0].NumBaseRows;
        int maxWidth = Buildings[0].NumCols;
        int maxHeight = Buildings[0].NumBaseRows;

        foreach (Building b in Buildings)
        {
            minStructureWidth = Mathf.Min(minStructureWidth, b.NumCols);
            minStructureHeight = Mathf.Min(minStructureHeight, b.NumBaseRows);
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

        maxBlockWidth = maxWidth + SizeCushionAmount;
        maxBlockHeight = maxHeight + SizeCushionAmount;

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
