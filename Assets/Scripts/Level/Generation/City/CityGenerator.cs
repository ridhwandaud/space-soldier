using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class CityGenerator : ILevelGenerator {

    public static List<Road> Roads = new List<Road>();
    public static List<Rect> FinalRectangles = new List<Rect>();
    public static List<Rect> StartingRects = new List<Rect>();
    private static int MaxDivisionsPerBase = 3;
    private static int MaxDivideAttempts = 2000;
    private static int MinDivideGap = 5;
    private static int MaxAttachAttemptsPerRect = 50;
    private static int NumStartingRectangles = 10;
    public static int PerimeterPadding = 6;

    public static Dictionary<PerimeterPoint, HashSet<PerimeterRect>> PointDict = new Dictionary<PerimeterPoint, HashSet<PerimeterRect>>();

    private static Int2 BaseRectWidthRange = new Int2(15, 15);
    private static Int2 BaseRectHeightRange = new Int2(10, 10);

    public static List<PerimeterRect> PerimeterRects = new List<PerimeterRect>();

    public int[,] GenerateLevel (int levelIndex, out Vector3 playerSpawn)
    {
        Rect startingRect = new Rect(0, 0, Random.Range(BaseRectWidthRange.x, BaseRectWidthRange.y),
            Random.Range(BaseRectHeightRange.x, BaseRectHeightRange.y));
        StartingRects.Add(startingRect);
        RenderRect(startingRect);
        Queue<Rect> q = new Queue<Rect>();
        List<Road> perimeterLines = new List<Road>();
        q.Enqueue(startingRect);

        AddRects(q, startingRect, NumStartingRectangles - 1);
        DrawPerimeter(perimeterLines);
        Vector3 playerSpawnRef = new Vector3() ;
        DivideRects(q, ref playerSpawnRef);

        playerSpawn = playerSpawnRef;
        new CityGridCreator().GenerateGrid(perimeterLines, FinalRectangles);

        return null;
    }

    void AddRects(Queue<Rect> q, Rect startingRect, int numToAdd)
    {
        AddPerimeterRect(new PerimeterRect(startingRect));
        Queue<Rect> attachmentAnchors = new Queue<Rect>();
        attachmentAnchors.Enqueue(startingRect);

        int numAdded = 0;

        while (numAdded < numToAdd)
        {
            int numAttemptsThisRect = 0;
            Rect curr = attachmentAnchors.Peek();

            while (numAttemptsThisRect < MaxAttachAttemptsPerRect)
            {
                int dirNum = Random.Range(0, 4); // 0 = left, 1 = top, 2 = right, 3 = bottom
                int width = Random.Range(BaseRectWidthRange.x, BaseRectWidthRange.y);
                int height = Random.Range(BaseRectHeightRange.x, BaseRectHeightRange.y);

                int x, y;

                if (dirNum == 0 || dirNum == 2)
                {
                    y = (int)Random.Range(curr.y, curr.y + curr.height);
                    x = (int)(dirNum == 0 ? curr.x - width : curr.x + curr.width);
                }
                else
                {
                    y = (int)(dirNum == 1 ? curr.y + curr.height : curr.y - height);
                    x = (int)Random.Range(curr.x, curr.x + curr.width);
                }

                Rect newRect = new Rect(x, y, width, height);
                bool overlap = false;

                foreach (Rect rect in StartingRects)
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
                    StartingRects.Add(newRect);
                    attachmentAnchors.Enqueue(newRect);
                    RenderRect(newRect);
                    AddPerimeterRect(new PerimeterRect(newRect));
                    numAdded++;
                    break;
                } else if(numAttemptsThisRect == MaxAttachAttemptsPerRect - 1)
                {
                    attachmentAnchors.Dequeue();
                }

                numAttemptsThisRect++;
            }
        }
    }

    void DrawPerimeter(List<Road> perimeterLines)
    {
        foreach (PerimeterRect r in PerimeterRects)
        {
            for (int i = 0; i < r.points.Count; i++)
            {
                PerimeterPoint curr = r.points[i];
                PerimeterPoint next = i == r.points.Count - 1 ? r.points[0] : r.points[i + 1];
                bool draw = true;

                if (curr.IsInsetPerimeter(next, r))
                {
                    draw = false;
                } else
                {
                    foreach (PerimeterRect potentialContainer in PerimeterRects)
                    {
                        if (potentialContainer.ContainsPoint(curr) || potentialContainer.ContainsPoint(next)
                            || IntersectsRect(curr, next))
                        {
                            draw = false;
                            break;
                        }
                    }
                }

                if (draw)
                {
                    perimeterLines.Add(new Road(curr.x, curr.y, next.x, next.y, true));
                }
            }
        }
    }

    public bool IntersectsRect(PerimeterPoint p1, PerimeterPoint p2)
    {
        bool vert = p1.y != p2.y;
        foreach (Rect r in StartingRects)
        {
            if (vert)
            {
                int x = p1.x, minY = Mathf.Min(p1.y, p2.y), maxY = Mathf.Max(p1.y, p2.y);
                if (x > r.xMin && x < r.xMax && ((r.yMin > minY && r.yMin < maxY) || (r.yMax > minY && r.yMax < maxY)))
                {
                    return true;
                }
            }
            else
            {
                int y = p1.y, minX = Mathf.Min(p1.x, p2.x), maxX = Mathf.Max(p1.x, p2.x);
                if (y > r.yMin && y < r.yMax && ((r.xMin > minX && r.xMin < maxX) || (r.xMax > minX && r.xMax < maxX)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void DivideRects(Queue<Rect> q, ref Vector3 playerSpawn)
    {
        int numDivisions = 0;
        int numAttempts = 0;
        int numInCurrentLevel = q.Count;
        int numInNextLevel = 0;
        bool horizontal = true;

        int maxDivisions = MaxDivisionsPerBase * q.Count;
        bool spawned = false;

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

                if (!spawned)
                {
                    spawned = true;
                    playerSpawn = new Vector3(Random.Range(curr.x, curr.xMax), newY);
                }
            }
            else if (!horizontal && curr.width > MinDivideGap * 2)
            {
                int newX = (int)(curr.x + Random.Range(MinDivideGap, curr.width - MinDivideGap));
                Roads.Add(new Road(newX, curr.y, newX, curr.yMax));
                q.Enqueue(new Rect(curr.x, curr.y, newX - curr.x, curr.height));
                q.Enqueue(new Rect(newX, curr.y, curr.xMax - newX, curr.height));
                numInNextLevel += 2;
                numDivisions++;

                if (!spawned)
                {
                    spawned = true;
                    playerSpawn = new Vector3(newX, Random.Range(curr.y, curr.yMax));
                }

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

        foreach (Rect r in q)
        {
            FinalRectangles.Add(r);
        }
    }

    void AddPerimeterRect(PerimeterRect r)
    {
        foreach (PerimeterRect other in PerimeterRects)
        {
            r.GenerateIntersections(other);
        }

        PerimeterRects.Add(r);
    }

    // TODO: Remove
    static void RenderRect(Rect rect, bool isPerim = false)
    {
        Roads.Add(new Road(rect.x, rect.y, rect.x, rect.yMax, isPerim));
        Roads.Add(new Road(rect.x, rect.y, rect.xMax, rect.y, isPerim));
        Roads.Add(new Road(rect.xMax, rect.y, rect.xMax, rect.yMax, isPerim));
        Roads.Add(new Road(rect.x, rect.yMax, rect.xMax, rect.yMax, isPerim));
    }
}
