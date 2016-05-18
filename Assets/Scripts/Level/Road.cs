using UnityEngine;
using System.Collections;

public class Road {

    public Vector2 Endpoint1;
    public Vector2 Endpoint2;
    public Side Side;

    // temporary
    private LineRenderer lineRenderer;

    public Road(float x1, float y1, float x2, float y2, int colorCode = 0, Side side = Side.Left)
    {
        Color col = Color.green;
        if (colorCode == 1)
        {
            col = Color.yellow;
        } else if (colorCode == 2)
        {
            col = Color.blue;
        }
        Endpoint1 = new Vector2(x1, y1);
        Endpoint2 = new Vector2(x2, y2);
        Side = side;
        GameObject someObj = MonoBehaviour.Instantiate(Resources.Load("RoadRenderer")) as GameObject;

        if (colorCode != 0)
        {
            lineRenderer = someObj.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, Endpoint1);
            lineRenderer.SetPosition(1, Endpoint2);

            lineRenderer.SetColors(col, col);
        }
    }

    // Roads are either completely horizontal or completely vertical.
    public bool IsHorizontal()
    {
        return Endpoint1.y == Endpoint2.y;
    }
}
