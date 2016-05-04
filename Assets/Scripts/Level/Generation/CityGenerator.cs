using UnityEngine;
using System.Collections.Generic;

public class CityGenerator : ILevelGenerator {

    public static List<Road> Roads = new List<Road>();
    public static List<Rect> FinalRectangles = new List<Rect>();
    private static float MinDistanceBetweenRoads = 5f;
    private static int MaxDivisions = 15;
    private static int MaxAttempts = 2000;
    private static int MinGap = 3;

    public int[,] GenerateLevel (int levelIndex, out Vector3 playerSpawn)
    {
        Rect startingRect = new Rect(0, 0, 20, 20);
        RenderRect(startingRect);
        Queue<Rect> q = new Queue<Rect>();
        q.Enqueue(startingRect);
        int numDivisions = 0;
        int numAttempts = 0;
        int numInCurrentLevel = 1;
        int numInNextLevel = 0;
        bool horizontal = true;

        while (q.Count > 0 && numAttempts < MaxAttempts && numDivisions < MaxDivisions)
        {
            Rect curr = q.Dequeue();
            if (horizontal && curr.height > MinGap * 2)
            {
                float newY = curr.y + Random.Range(MinGap, curr.height - MinGap / 2);
                Roads.Add(new Road(curr.x, newY, curr.xMax, newY));
                q.Enqueue(new Rect(curr.x, curr.y, curr.width, newY - curr.y));
                q.Enqueue(new Rect(curr.x, newY, curr.width, curr.yMax - newY));
                numInNextLevel += 2;
                numDivisions++;
            } else if (!horizontal && curr.width > MinGap * 2)
            {
                float newX = curr.x + Random.Range(MinGap, curr.width - MinGap / 2);
                Roads.Add(new Road(newX, curr.y, newX, curr.yMax));
                q.Enqueue(new Rect(curr.x, curr.y, newX - curr.x, curr.height));
                q.Enqueue(new Rect(newX, curr.y, curr.xMax - newX, curr.height));
                numInNextLevel += 2;
                numDivisions++;
            } else
            {
                FinalRectangles.Add(curr);
            }

            numInCurrentLevel--;

            if (numInCurrentLevel == 0)
            {
                numInCurrentLevel = numInNextLevel;
                numInNextLevel = 0;
                horizontal = !horizontal;
            }
            numAttempts++;
        }

        playerSpawn = Vector3.zero;
        return null;
    }

    void RenderRect(Rect rect)
    {
        Roads.Add(new Road(rect.x, rect.y, rect.x, rect.yMax));
        Roads.Add(new Road(rect.x, rect.y, rect.xMax, rect.y));
        Roads.Add(new Road(rect.xMax, rect.y, rect.xMax, rect.yMax));
        Roads.Add(new Road(rect.x, rect.yMax, rect.xMax, rect.yMax));
    }
}
