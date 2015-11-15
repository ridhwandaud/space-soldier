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
        // GetComponentInParent doesn't only look at the immediate parent - keeps going up till it finds one.
        canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
    }

	public void OnPointerDown (PointerEventData data)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(tileRectTransform, data.position, data.pressEventCamera, out pointerOffset);
    }

    public void OnDrag (PointerEventData data)
    {
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, data.position, data.pressEventCamera, out localPointerPosition);

        tileRectTransform.localPosition = localPointerPosition - pointerOffset;
    }

    public void OnPointerUp (PointerEventData data)
    {
        transform.position = SnapPosition;
    }
}
