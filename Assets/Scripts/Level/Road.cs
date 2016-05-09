using UnityEngine;
using System.Collections;

public class Road {

    public Vector2 Endpoint1;
    public Vector2 Endpoint2;

    // temporary
    private LineRenderer lineRenderer;

    public Road(float x1, float y1, float x2, float y2, bool isPerim = false)
    {
        Color col = isPerim ? Color.yellow : Color.green;
        Endpoint1 = new Vector2(x1, y1);
        Endpoint2 = new Vector2(x2, y2);
        //GameObject someObj = MonoBehaviour.Instantiate(Resources.Load("RoadRenderer")) as GameObject;
        //lineRenderer = someObj.GetComponent<LineRenderer>();
        //lineRenderer.SetPosition(0, Endpoint1);
        //lineRenderer.SetPosition(1, Endpoint2);
        //lineRenderer.SetColors(col, col);
    }

    // Roads are either completely horizontal or completely vertical.
    public bool IsHorizontal()
    {
        return Endpoint1.y == Endpoint2.y;
    }
}
