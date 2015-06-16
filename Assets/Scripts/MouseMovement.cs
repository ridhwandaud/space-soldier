using UnityEngine;
using System.Collections;

public class MouseMovement : MonoBehaviour {

	void Update () {
        Vector3 mouse = Input.mousePosition;
        // WorldToScreenPoint converts a point from world space into screen space (defined in pixels, bottom-left of screen is (0,0)).
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
        var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
	}
}
