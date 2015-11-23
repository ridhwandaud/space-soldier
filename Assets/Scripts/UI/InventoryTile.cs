using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryTile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    private Transform slotTransform;
    private Vector2 pointerOffset;
    private RectTransform tileRectTransform;
    private RectTransform canvasRectTransform;
    private List<RectTransform> slotRects;
    private float rectWidth;
    private float rectHeight;
    private RectTransform mostOverlappedRect;

    public void Init(Transform slotTransform, List<RectTransform> slotRects)
    {
        this.slotTransform = slotTransform;
        this.slotRects = slotRects;
        tileRectTransform = transform as RectTransform;
        canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;

        Vector3[] corners = new Vector3[4];
        tileRectTransform.GetWorldCorners(corners);
        // RectTransform doesn't give a way to get width/height in world units, so must hack it.
        rectWidth = corners[2].x - corners[0].x;
        rectHeight = corners[2].y - corners[0].y;
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
        HandleTileOverlap();
    }

    void HandleTileOverlap()
    {
        float maxArea = 0;
        mostOverlappedRect = null;
        for (int i = 0; i < slotRects.Count; i++)
        {
            RectTransform rect = slotRects[i];
            // Could probably optimize this by caching the previous rect but... fuck it. Performs fine already.
            rect.GetComponent<Image>().color = Color.grey;
            if (BoxesCollide(rect, tileRectTransform))
            {
                float collisionArea = CollisionArea(rect, tileRectTransform);
                if (collisionArea > maxArea)
                {
                    maxArea = collisionArea;
                    mostOverlappedRect = rect;
                }
            }
        }

        if (mostOverlappedRect)
        {
            mostOverlappedRect.GetComponent<Image>().color = Color.green;
        }
    }

    public void OnPointerUp (PointerEventData data)
    {
        if (mostOverlappedRect && !mostOverlappedRect.GetComponent<InventorySlot>().Occupied)
        {
            slotTransform.GetComponent<InventorySlot>().Occupied = false;
            slotTransform = mostOverlappedRect.transform;
            slotTransform.GetComponent<InventorySlot>().Occupied = true;
        }

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

    void OnTriggerEnter2D(Collider2D other)
    {
        print("colliding with " + other.tag);
    }

    private float CollisionArea(RectTransform box1, RectTransform box2)
    {
        // Can check centers instead of edges because these are all squares of the same size.
        float xMax = box1.position.x > box2.position.x ? box2.position.x + rectWidth / 2 : box1.position.x + rectWidth / 2;
        float xMin = box1.position.x > box2.position.x ? box1.position.x - rectWidth / 2 : box2.position.x - rectWidth / 2;
        float yMax = box1.position.y > box2.position.y ? box2.position.y + rectHeight / 2 : box1.position.y + rectHeight / 2;
        float yMin = box1.position.y > box2.position.y ? box1.position.y - rectHeight / 2 : box2.position.y - rectHeight / 2;

        return (xMax - xMin) * (yMax - yMin);
    }

    private bool BoxesCollide(RectTransform box1, RectTransform box2)
    {
        return Mathf.Abs(box1.position.x - box2.position.x) < (rectWidth + rectWidth) / 2 &&
            Mathf.Abs(box1.position.y - box2.position.y) < (rectHeight + rectHeight) / 2;
    }
}
