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
    private static int NumStartingRectangles = 5;
    private static int PerimeterPadding = 3;

    private static Int2 BaseRectWidthRange = new Int2(15, 20);
    private static Int2 BaseRectHeightRange = BaseRectWidthRange;

    public static List<PerimeterRect> PerimeterRects = new List<PerimeterRect>();

    public int[,] GenerateLevel (int levelIndex, out Vector3 playerSpawn)
    {
        Rect startingRect = new Rect(0, 0, Random.Range(BaseRectWidthRange.x, BaseRectWidthRange.y),
            Random.Range(BaseRectHeightRange.x, BaseRectHeightRange.y));
        RenderRect(startingRect);
        Queue<Rect> q = new Queue<Rect>();
        q.Enqueue(startingRect);

        AddRects(q, startingRect, NumStartingRectangles - 1);
        DrawPerimeter();
        DivideRects(q);

        playerSpawn = Vector3.zero;
        return null;
    }

    void AddRects(Queue<Rect> q, Rect startingRect, int numToAdd)
    {
        Rect curr = startingRect;
        AddPerimeterRect(new PerimeterRect(startingRect));
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
                AddPerimeterRect(new PerimeterRect(newRect));
                numAdded++;
            }

            numAttempts++;
        }
    }

    // TODO: Make sure that perimeters don't pass through existing rects. Also ignore any intersections
    // that are shared by three or more rectangles.
    void DrawPerimeter()
    {
        foreach (PerimeterRect r in PerimeterRects)
        {
            for (int i = 0; i < r.points.Count; i++)
            {
                PerimeterPoint curr = r.points[i];
                PerimeterPoint next = i == r.points.Count - 1 ? r.points[0] : r.points[i + 1];
                bool contained = false;

                foreach (PerimeterRect potentialContainer in PerimeterRects)
                {
                    if (potentialContainer.ContainsPoint(curr) || potentialContainer.ContainsPoint(next)) {
                        contained = true;
                        break;
                    }
                }

                if (!contained)
                {
                    new Road(curr.x, curr.y, next.x, next.y, true);
                }
            }
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

    void AddPerimeterRect(PerimeterRect r)
    {
        foreach (PerimeterRect other in PerimeterRects)
        {
            r.GenerateIntersections(other);
        }

        PerimeterRects.Add(r);
    }

    static void RenderRect(Rect rect, bool isPerim = false)
    {
        Roads.Add(new Road(rect.x, rect.y, rect.x, rect.yMax, isPerim));
        Roads.Add(new Road(rect.x, rect.y, rect.xMax, rect.y, isPerim));
        Roads.Add(new Road(rect.xMax, rect.y, rect.xMax, rect.yMax, isPerim));
        Roads.Add(new Road(rect.x, rect.yMax, rect.xMax, rect.yMax, isPerim));
    }

    public class PerimeterRect
    {
        public List<PerimeterPoint> points;
        public float top;
        public float right;
        public float bottom;
        public float left;

        public PerimeterRect (Rect r)
        {
            points = new List<PerimeterPoint>();

            top = r.yMax + PerimeterPadding;
            right = r.xMax + PerimeterPadding;
            bottom = r.yMin - PerimeterPadding;
            left = r.xMin - PerimeterPadding;

            points.Add(new PerimeterPoint(left, top, Side.Top));
            points.Add(new PerimeterPoint(right, top, Side.Right));
            points.Add(new PerimeterPoint(right, bottom, Side.Bottom));
            points.Add(new PerimeterPoint(left, bottom, Side.Left));
        }

        public void AddPoint(PerimeterPoint p)
        {
            int numPoints = points.Count;
            for (int x = 0; x <= numPoints; x++)
            {
                if (x == numPoints)
                {
                    points.Add(p);
                }
                else if (p.CompareTo(points[x]) < 0)
                {
                    points.Insert(x, p);
                    break;
                }
            }
        }

        public bool ContainsPoint(PerimeterPoint p)
        {
            return p.x > left && p.x < right && p.y > bottom && p.y < top;
        }

        public void GenerateIntersections(PerimeterRect other)
        {
            if (right > other.left && right < other.right)
            {
                if (other.top > bottom && other.top < top)
                {
                    AddPoint(new PerimeterPoint(right, other.top, Side.Right));
                    other.AddPoint(new PerimeterPoint(right, other.top, Side.Top));
                }

                if (other.bottom > bottom && other.bottom < top)
                {
                    AddPoint(new PerimeterPoint(right, other.bottom, Side.Right));
                    other.AddPoint(new PerimeterPoint(right, other.bottom, Side.Bottom));
                }
            }

            if (left > other.left && left < other.right)
            {
                if (other.top > bottom && other.top < top)
                {
                    AddPoint(new PerimeterPoint(left, other.top, Side.Left));
                    other.AddPoint(new PerimeterPoint(left, other.top, Side.Top));
                }

                if (other.bottom > bottom && other.bottom < top)
                {
                    AddPoint(new PerimeterPoint(left, other.bottom, Side.Left));
                    other.AddPoint(new PerimeterPoint(left, other.bottom, Side.Bottom));
                }
            }

            if (bottom > other.bottom && bottom < other.top)
            {
                if (other.left > left && other.left < right)
                {
                    AddPoint(new PerimeterPoint(other.left, bottom, Side.Bottom));
                    other.AddPoint(new PerimeterPoint(other.left, bottom, Side.Left));
                }

                if (other.right > left && other.right < right)
                {
                    AddPoint(new PerimeterPoint(other.right, bottom, Side.Bottom));
                    other.AddPoint(new PerimeterPoint(other.right, bottom, Side.Right));
                }
            }

            if (top > other.bottom && top < other.top)
            {
                if (other.left > left && other.left < right)
                {
                    AddPoint(new PerimeterPoint(other.left, top, Side.Top));
                    other.AddPoint(new PerimeterPoint(other.left, top, Side.Left));
                }

                if (other.right > left && other.right < right)
                {
                    AddPoint(new PerimeterPoint(other.right, top, Side.Top));
                    other.AddPoint(new PerimeterPoint(other.right, top, Side.Right));
                }
            }
        }
    }

    public class PerimeterPoint
    {
        public float x;
        public float y;
        public Side side;

        public PerimeterPoint(float x, float y, Side side)
        {
            this.x = x;
            this.y = y;
            this.side = side;
        }

        public int CompareTo (PerimeterPoint other)
        {
            if (side.CompareTo(other.side) != 0)
            {
                return side.CompareTo(other.side);
            }

            if (side.Equals(Side.Top))
            {
                return x.CompareTo(other.x);
            }
            else if (side.Equals(Side.Right))
            {
                return other.y.CompareTo(y);
            }
            else if (side.Equals(Side.Bottom))
            {
                return other.x.CompareTo(x);
            }
            else
            {
                return y.CompareTo(other.y);
            }
        }
    }

    public enum Side
    {
        Top = 0, Right = 1, Bottom = 2, Left = 3
    }
}
