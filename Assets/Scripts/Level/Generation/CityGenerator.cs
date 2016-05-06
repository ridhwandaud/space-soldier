using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class CityGenerator : ILevelGenerator {

    public static List<Road> Roads = new List<Road>();
    public static List<Rect> FinalRectangles = new List<Rect>();
    private static int MaxDivisionsPerBase = 8;
    private static int MaxDivideAttempts = 2000;
    private static int MinDivideGap = 4;
    private static int MaxAttachAttempts = 1000;
    private static int NumStartingRectangles = 4;

    private static Int2 BaseRectWidthRange = new Int2(15, 20);
    private static Int2 BaseRectHeightRange = BaseRectWidthRange;

    public int[,] GenerateLevel (int levelIndex, out Vector3 playerSpawn)
    {
        Rect startingRect = new Rect(0, 0, Random.Range(BaseRectWidthRange.x, BaseRectWidthRange.y),
            Random.Range(BaseRectHeightRange.x, BaseRectHeightRange.y));
        RenderRect(startingRect);
        Queue<Rect> q = new Queue<Rect>();
        q.Enqueue(startingRect);

        AddRects(q, startingRect, NumStartingRectangles - 1);
        DivideRects(q);

        playerSpawn = Vector3.zero;
        return null;
    }

    void AddRects(Queue<Rect> q, Rect startingRect, int numToAdd)
    {
        Rect curr = startingRect;
        List<Rect> rects = new List<Rect>();

        int numAdded = 0;
        int numAttempts = 0;

        while (numAdded < numToAdd && numAttempts < MaxAttachAttempts)
        {
            int dirNum = Random.Range(0, 4); // 0 = left, 1 = top, 2 = right, 3 = bottom
            int width = Random.Range(BaseRectWidthRange.x, BaseRectWidthRange.y);
            int height = Random.Range(BaseRectHeightRange.x, BaseRectHeightRange.y);

            int x, y;

            if (dirNum == 0 || dirNum == 2)
            {
                y = (int)Random.Range(curr.y , curr.y + curr.height);
                x = (int)(dirNum == 0 ? curr.x - width : curr.x + curr.width);
            } else
            {
                y = (int)(dirNum == 1 ? curr.y + curr.height  : curr.y - height);
                x = (int)Random.Range(curr.x, curr.x + curr.width);
            }

            Rect newRect = new Rect(x, y, width, height);
            bool overlap = false;

            foreach ( Rect rect in rects)
            {
                if (newRect.Overlaps(rect))
                {
                    overlap = true;
                    break;
                }
            }

            if (!overlap)
            {
                q.Enqueue(newRect);
                rects.Add(newRect);
                RenderRect(newRect);
                numAdded++;
            }

            numAttempts++;
        }
    }

    void DivideRects(Queue<Rect> q)
    {
        int numDivisions = 0;
        int numAttempts = 0;
        int numInCurrentLevel = q.Count;
        int numInNextLevel = 0;
        bool horizontal = true;

        int maxDivisions = MaxDivisionsPerBase * q.Count;

        while (q.Count > 0 && numAttempts < MaxDivideAttempts && numDivisions < maxDivisions)
        {
            Rect curr = q.Dequeue();
            if (horizontal && curr.height > MinDivideGap * 2)
            {
                int newY = (int)(curr.y + Random.Range(MinDivideGap, curr.height - MinDivideGap));
                Roads.Add(new Road(curr.x, newY, curr.xMax, newY));
                q.Enqueue(new Rect(curr.x, curr.y, curr.width, newY - curr.y));
                q.Enqueue(new Rect(curr.x, newY, curr.width, curr.yMax - newY));
                numInNextLevel += 2;
                numDivisions++;
            }
            else if (!horizontal && curr.width > MinDivideGap * 2)
            {
                int newX = (int)(curr.x + Random.Range(MinDivideGap, curr.width - MinDivideGap));
                Roads.Add(new Road(newX, curr.y, newX, curr.yMax));
                q.Enqueue(new Rect(curr.x, curr.y, newX - curr.x, curr.height));
                q.Enqueue(new Rect(newX, curr.y, curr.xMax - newX, curr.height));
                numInNextLevel += 2;
                numDivisions++;
            }
            else
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
    }

    void RenderRect(Rect rect)
    {
        Roads.Add(new Road(rect.x, rect.y, rect.x, rect.yMax));
        Roads.Add(new Road(rect.x, rect.y, rect.xMax, rect.y));
        Roads.Add(new Road(rect.xMax, rect.y, rect.xMax, rect.yMax));
        Roads.Add(new Road(rect.x, rect.yMax, rect.xMax, rect.yMax));
    }
}
