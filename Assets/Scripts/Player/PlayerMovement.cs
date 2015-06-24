using UnityEngine;
using System.Collections;
using SpriteTile;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D rb;

    public float speed = 8;
    public float collisionDistance;

    private static int WALL_LAYER = 1;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector2 newVelocity = new Vector2(inputX, inputY).normalized * speed;

        if (Tile.GetCollider(new Vector2(rb.position.x, rb.position.y + collisionDistance), WALL_LAYER)
            && inputY > 0)
        {
            newVelocity.y = 0;
        }
        if (Tile.GetCollider(new Vector2(rb.position.x, rb.position.y - collisionDistance), WALL_LAYER)
            && inputY < 0)
        {
            newVelocity.y = 0;
        }
        if (Tile.GetCollider(new Vector2(rb.position.x + collisionDistance, rb.position.y), WALL_LAYER)
            && inputX > 0)
        {
            newVelocity.x = 0;
        }
        if (Tile.GetCollider(new Vector2(rb.position.x - collisionDistance, rb.position.y), WALL_LAYER)
            && inputX < 0)
        {
            newVelocity.x = 0;
        }

        rb.velocity = newVelocity;
    }
}
