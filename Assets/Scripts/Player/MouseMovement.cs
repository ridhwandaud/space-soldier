using UnityEngine;

public class MouseMovement : MonoBehaviour {

    private Animator animator;

    void Awake ()
    {
        animator = GetComponent<Animator>();
    }

	void Update () {
        if (GameState.Paused)
        {
            return;
        }

        Vector2 offset = VectorUtil.DirectionToMousePointer(transform);
        var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg - 90;
        // Do this with the weapon
        // transform.rotation = Quaternion.Euler(0, 0, angle);
        animator.SetFloat("MouseOffsetX", offset.x);
	}
}
