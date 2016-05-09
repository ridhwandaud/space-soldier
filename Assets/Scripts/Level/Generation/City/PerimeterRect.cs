using UnityEngine;
using System.Collections.Generic;

public class PerimeterRect
{
    public List<PerimeterPoint> points;
    public int top;
    public int right;
    public int bottom;
    public int left;

    public PerimeterRect (Rect r)
    {
        points = new List<PerimeterPoint>();

        top = (int)r.yMax + CityGenerator.PerimeterPadding;
        right = (int)r.xMax + CityGenerator.PerimeterPadding;
        bottom = (int)r.yMin - CityGenerator.PerimeterPadding;
        left = (int)r.xMin - CityGenerator.PerimeterPadding;

        AddPoint(new PerimeterPoint(left, top, Side.Top));
        AddPoint(new PerimeterPoint(right, top, Side.Right));
        AddPoint(new PerimeterPoint(right, bottom, Side.Bottom));
        AddPoint(new PerimeterPoint(left, bottom, Side.Left));
    }

    public bool Overlaps (PerimeterRect other)
    {
        return new Rect(left, bottom, right - left, top - bottom).Overlaps(new Rect(other.left, other.bottom,
            other.right - other.left, other.top - other.bottom));
    }

    public void AddPoint (PerimeterPoint p)
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

        if (CityGenerator.PointDict.ContainsKey(p))
        {
            CityGenerator.PointDict[p].Add(this);
        }
        else
        {
            CityGenerator.PointDict[p] = new HashSet<PerimeterRect> { this };
        }
    }

    public bool ContainsPoint (PerimeterPoint p)
    {
        return p.x > left && p.x < right && p.y > bottom && p.y < top;
    }

    public void GenerateIntersections (PerimeterRect other)
    {
        if (right >= other.left && right <= other.right)
        {
            if (other.top >= bottom && other.top <= top)
            {
                AddPoint(new PerimeterPoint(right, other.top, Side.Right));
                other.AddPoint(new PerimeterPoint(right, other.top, Side.Top));
            }

            if (other.bottom >= bottom && other.bottom <= top)
            {
                AddPoint(new PerimeterPoint(right, other.bottom, Side.Right));
                other.AddPoint(new PerimeterPoint(right, other.bottom, Side.Bottom));
            }
        }

        if (left >= other.left && left <= other.right)
        {
            if (other.top >= bottom && other.top <= top)
            {
                AddPoint(new PerimeterPoint(left, other.top, Side.Left));
                other.AddPoint(new PerimeterPoint(left, other.top, Side.Top));
            }

            if (other.bottom >= bottom && other.bottom <= top)
            {
                AddPoint(new PerimeterPoint(left, other.bottom, Side.Left));
                other.AddPoint(new PerimeterPoint(left, other.bottom, Side.Bottom));
            }
        }

        if (bottom >= other.bottom && bottom <= other.top)
        {
            if (other.left >= left && other.left <= right)
            {
                AddPoint(new PerimeterPoint(other.left, bottom, Side.Bottom));
                other.AddPoint(new PerimeterPoint(other.left, bottom, Side.Left));
            }

            if (other.right >= left && other.right <= right)
            {
                AddPoint(new PerimeterPoint(other.right, bottom, Side.Bottom));
                other.AddPoint(new PerimeterPoint(other.right, bottom, Side.Right));
            }
        }

        if (top >= other.bottom && top <= other.top)
        {
            if (other.left >= left && other.left <= right)
            {
                AddPoint(new PerimeterPoint(other.left, top, Side.Top));
                other.AddPoint(new PerimeterPoint(other.left, top, Side.Left));
            }

            if (other.right >= left && other.right <= right)
            {
                AddPoint(new PerimeterPoint(other.right, top, Side.Top));
                other.AddPoint(new PerimeterPoint(other.right, top, Side.Right));
            }
        }
    }
}