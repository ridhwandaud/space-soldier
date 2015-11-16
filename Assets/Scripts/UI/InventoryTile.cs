using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    private Transform slotTransform;
    private Vector2 pointerOffset;
    private RectTransform tileRectTransform;
    private RectTransform canvasRectTransform;

    public void Init(Transform slotTransform)
    {
        this.slotTransform = slotTransform;
        tileRectTransform = transform as RectTransform;
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
        transform.position = slotTransform.position;
    }

    Vector2 ClampToWindow (Vector2 pos)
    {
        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetLocalCorners(canvasCorners); // 0: lower left, 1: upper left, 2: upper right, 3: lower right

        float halfTileLength = InventoryManager.TileSideLength / 2;

        float clampedX = Mathf.Clamp(pos.x, canvasCorners[0].x + halfTileLength, canvasCorners[2].x - halfTileLength);
        float clampedY = Mathf.Clamp(pos.y, canvasCorners[0].y + halfTileLength, canvasCorners[2].y - halfTileLength);

        return new Vector2(clampedX, clampedY);
    }
}
