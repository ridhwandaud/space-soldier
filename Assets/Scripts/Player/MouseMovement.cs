using UnityEngine;

public class MouseMovement : MonoBehaviour {

    private Animator animator;

    void Awake ()
    {
        animator = GetComponent<Animator>();
    }

	void Update () {
        if (GameState.Paused || GameState.InputLocked)
        {
            return;
        }

        Vector2 offset = VectorUtil.DirectionToMousePointer(transform);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        Player.PlayerWeaponControl.RotateWeapon(angle);
        animator.SetFloat("MouseOffsetX", offset.x);
	}
}
