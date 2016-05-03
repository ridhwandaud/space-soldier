using UnityEngine;
using System.Collections.Generic;

public class CityGenerator : ILevelGenerator {

    // Temporary
    public static List<Road> Roads = new List<Road>();

    public int[,] GenerateLevel(int levelIndex, out Vector3 playerSpawn)
    {
        List<Road> roads = new List<Road>();
        roads.Add(new Road(0, 0, 0, 5));

        Queue<Road> q = new Queue<Road>();
        q.Enqueue(roads[0]);

        //int numAttempts = 0;
        int generatedRoadCount = 0;

        while (q.Count > 0 && roads.Count < 50)
        {
            Road road = q.Dequeue();
            generatedRoadCount = 0;
            while (generatedRoadCount < 3)
            {
                Road generatedRoad = GenerateRoad(road);
                roads.Add(generatedRoad);
                q.Enqueue(generatedRoad);
                generatedRoadCount++;
            }
        }

        playerSpawn = Vector3.zero;
        return null;
    }

    Road GenerateRoad(Road road)
    {
        int sign = Random.Range(0, 1) < 0 ? -1 : 1;
        int length = Random.Range(3, 12);

        if (road.IsHorizontal())
        {
            Vector2 endpoint1 = new Vector2(Random.Range(road.Endpoint1.x, road.Endpoint2.x), road.Endpoint1.y);
            Vector2 endpoint2 = new Vector2(endpoint1.x, endpoint1.y + length * sign);
            return new Road(endpoint1.x, endpoint1.y, endpoint2.x, endpoint2.y);
        } else
        {
            Vector2 endpoint1 = new Vector2(road.Endpoint1.x, Random.Range(road.Endpoint1.y, road.Endpoint2.y));
            Vector2 endpoint2 = new Vector2(endpoint1.x + length * sign, endpoint1.y);
            return new Road(endpoint1.x, endpoint1.y, endpoint2.x, endpoint2.y);
        }
    }
}
