using UnityEngine;
using System.Collections;

public class MouseMovement : MonoBehaviour {

	void Update () {
        Vector2 offset = VectorUtil.DirectionToMousePointer(transform);
        var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
	}
}
