using UnityEngine;
using System.Collections.Generic;

public class BranchingCityGenerator : ILevelGenerator {

    public static List<Road> Roads = new List<Road>();
    private static float MinDistanceBetweenRoads = 5f;

    public int[,] GenerateLevel(int levelIndex, out Vector3 playerSpawn)
    {
        Algorithm1();

        playerSpawn = Vector3.zero;
        return null;
    }

    void Algorithm1()
    {
        Roads.Add(new Road(0, 0, 0, 15));

        Queue<Road> q = new Queue<Road>();
        q.Enqueue(Roads[0]);

        int numAttempts = 0;
        int totNumAttemptsPerRoad = 20;
        int totNumRoads = 25;
        int generatedRoadCount = 0;

        while (q.Count > 0 && Roads.Count < totNumRoads)
        {
            Road road = q.Dequeue();
            generatedRoadCount = 0;
            int numAttemptsThisRoad = 0;

            while (generatedRoadCount < 3 && numAttemptsThisRoad < totNumAttemptsPerRoad)
            {
                numAttempts++;
                numAttemptsThisRoad++;
                Road generatedRoad = GenerateRoad(road);
                if (generatedRoad != null)
                {
                    Roads.Add(generatedRoad);
                    q.Enqueue(generatedRoad);
                    generatedRoadCount++;
                }
            }
        }
    }

    Road GenerateRoad(Road road)
    {
        int sign = Random.Range(0, 2) == 0 ? -1 : 1;
        int length = Random.Range(5, 20);
        if (road.IsHorizontal())
        {
            int x = (int)Random.Range(road.Endpoint1.x, road.Endpoint2.x);
            Vector2 endpoint1 = new Vector2(x, road.Endpoint1.y);
            Vector2 endpoint2 = new Vector2(endpoint1.x, endpoint1.y + length * sign);
            if (!TooCloseToExistingRoad(false, endpoint1, endpoint2))
            {
                return new Road(endpoint1.x, endpoint1.y, endpoint2.x, endpoint2.y);
            }
        } else
        {
            int y = (int)Random.Range(road.Endpoint1.y, road.Endpoint2.y);
            Vector2 endpoint1 = new Vector2(road.Endpoint1.x, y);
            Vector2 endpoint2 = new Vector2(endpoint1.x + length * sign, endpoint1.y);
            if (!TooCloseToExistingRoad(true, endpoint1, endpoint2))
            {
                return new Road(endpoint1.x, endpoint1.y, endpoint2.x, endpoint2.y);
            }
        }

        return null;
    }

    bool TooCloseToExistingRoad(bool isHorizontal, Vector2 endpoint1, Vector2 endpoint2)
    {
        foreach (Road otherRoad in Roads)
        {
            if (isHorizontal)
            {
                if (otherRoad.IsHorizontal() 
                    && (IsBetween(endpoint1.x, otherRoad.Endpoint1.x, otherRoad.Endpoint2.x)
                        || IsBetween(endpoint2.x, otherRoad.Endpoint1.x, otherRoad.Endpoint2.x)
                        || IsBetween(otherRoad.Endpoint1.x, endpoint1.x, endpoint2.x)
                        || IsBetween(otherRoad.Endpoint2.x, endpoint1.x, endpoint2.x)
                        || (endpoint1.x == otherRoad.Endpoint1.x && endpoint2.x == otherRoad.Endpoint2.x))
                    && Mathf.Abs(endpoint1.y - otherRoad.Endpoint1.y) < MinDistanceBetweenRoads) {
                    return true;
                }
            } else
            {
                if (!otherRoad.IsHorizontal()
                    && (IsBetween(endpoint1.y, otherRoad.Endpoint1.y, otherRoad.Endpoint2.y)
                        || IsBetween(endpoint2.y, otherRoad.Endpoint1.y, otherRoad.Endpoint2.y)
                        || IsBetween(otherRoad.Endpoint1.y, endpoint1.y, endpoint2.y)
                        || IsBetween(otherRoad.Endpoint2.y, endpoint1.y, endpoint2.y)
                        || (endpoint1.y == otherRoad.Endpoint1.y && endpoint2.y == otherRoad.Endpoint2.y))
                    && Mathf.Abs(endpoint1.x - otherRoad.Endpoint1.x) < MinDistanceBetweenRoads)
                {
                    return true;
                }

            }
        }

        return false;
    }

    bool IsBetween(float valToCheck, float bound1, float bound2)
    {
        float min = bound1 < bound2 ? bound1 : bound2;
        float max = bound1 < bound2 ? bound2 : bound1;

        return (valToCheck > min && valToCheck < max) && valToCheck != min && valToCheck != max;
    }
}
