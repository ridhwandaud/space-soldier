using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    public Vector2 SnapPosition;

    private Vector2 pointerOffset;
    private RectTransform tileRectTransform;
    private RectTransform canvasRectTransform;

    public void Init()
    {
        SnapPosition = transform.position;
        tileRectTransform = transform as RectTransform;
        // GetComponentInParent doesn't only look at the immediate parent.
        canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
    }

	public void OnPointerDown (PointerEventData data)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(tileRectTransform, data.position, data.pressEventCamera, out pointerOffset);
    }

    public void OnDrag (PointerEventData data)
    {
        Vector2 localPointerPosition;
        // Canvas rectangle != Screen rectangle.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, data.position, data.pressEventCamera, out localPointerPosition);

        tileRectTransform.localPosition = ClampToWindow(localPointerPosition - pointerOffset);

    }

    public void OnPointerUp (PointerEventData data)
    {
        transform.position = SnapPosition;
    }

    Vector2 ClampToWindow (Vector2 pos)
    {
        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetLocalCorners(canvasCorners); // 0: lower left, 1: upper left, 2: upper right, 3: lower right

        float clampedX = Mathf.Clamp(pos.x, canvasCorners[0].x + 30, canvasCorners[2].x - 30);
        float clampedY = Mathf.Clamp(pos.y, canvasCorners[0].y + 30, canvasCorners[2].y - 30);

        return new Vector2(clampedX, clampedY);
    }
}
