using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class CityGenerator : ILevelGenerator {

    private CityAlgorithm CityAlgorithm = new CityAlgorithm();
    private CityDecorator CityDecorator = new CityDecorator();

    public int[,] GenerateLevel (int levelIndex, out Vector3 playerSpawn)
    {
        List<Rect> finalRectangles = new List<Rect>();
        List<Road> perimeterLines = new List<Road>();
        int[,] grid = CityAlgorithm.ExecuteAlgorithm(finalRectangles, perimeterLines, out playerSpawn);
        CityDecorator.GenerateBuildings(finalRectangles, grid);
        CityDecorator.DecoratePerimeters(perimeterLines, grid);

        return grid;
    }
}
