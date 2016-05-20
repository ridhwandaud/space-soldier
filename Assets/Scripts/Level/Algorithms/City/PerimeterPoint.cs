using System.Collections.Generic;

public class PerimeterPoint
{
    public int x;
    public int y;
    public Side side;

    public PerimeterPoint (int x, int y, Side side)
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

    // This method checks to see if both points belong to the same 2 or more non-overlapping rects. If they do, then it does
    // not draw the line because these types of lines are not truly part of the perimeter and they make the whole city look kind of weird.
    public bool IsInsetPerimeter (PerimeterPoint other, PerimeterRect thisRect)
    {
        HashSet<PerimeterRect> rectsContainingThisPoint = CityAlgorithm.PointDict[this];
        HashSet<PerimeterRect> rectsContainingTargetPoint = CityAlgorithm.PointDict[other];
        List<PerimeterRect> sharedRects = new List<PerimeterRect>();

        foreach (PerimeterRect rect in rectsContainingThisPoint)
        {
            if (rectsContainingTargetPoint.Contains(rect) && rect != thisRect)
            {
                sharedRects.Add(rect);
            }
        }

        if (sharedRects.Count < 1)
        {
            return false;
        }

        foreach (PerimeterRect r in sharedRects)
        {
            if (r.Overlaps(thisRect))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode ()
    {
        return 1000 * x + y;
    }

    public override bool Equals (object point)
    {
        PerimeterPoint actualPoint = point as PerimeterPoint;
        return x == actualPoint.x && y == actualPoint.y;
    }
}