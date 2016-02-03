using UnityEngine;
using SpriteTile;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    public float speed = 8;
    public float collisionDistance;
    public int wallSpriteTileLayer = 1;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
	}

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        float halfCollisionDistance = collisionDistance / 2;
 
        if(GameState.TutorialMode && (inputX != 0 || inputY != 0))
        {
            TutorialEngine.Instance.Trigger(TutorialTrigger.Walk);
        }

        Vector2 newVelocity = new Vector2(inputX, inputY).normalized * speed;

        if ((Tile.GetCollider(new Vector2(rb.position.x, rb.position.y + collisionDistance), wallSpriteTileLayer)
            || Tile.GetCollider(new Vector2(rb.position.x - halfCollisionDistance, rb.position.y + collisionDistance), wallSpriteTileLayer)
            || Tile.GetCollider(new Vector2(rb.position.x + halfCollisionDistance, rb.position.y + collisionDistance), wallSpriteTileLayer)
            || Physics2D.BoxCast(rb.position, boxCollider.size, 0f, Vector2.up, halfCollisionDistance,
                LayerMasks.ObstacleLayerMask).transform != null)
            && inputY > 0)
        {
            newVelocity.y = 0;
        }
        if ((Tile.GetCollider(new Vector2(rb.position.x, rb.position.y - collisionDistance), wallSpriteTileLayer)
            || Tile.GetCollider(new Vector2(rb.position.x - halfCollisionDistance, rb.position.y - collisionDistance), wallSpriteTileLayer)
            || Tile.GetCollider(new Vector2(rb.position.x + halfCollisionDistance, rb.position.y - collisionDistance), wallSpriteTileLayer)
            || Physics2D.BoxCast(rb.position, boxCollider.size, 0f, Vector2.down, halfCollisionDistance,
                LayerMasks.ObstacleLayerMask).transform != null)
            && inputY < 0)
        {
            newVelocity.y = 0;
        }
        if ((Tile.GetCollider(new Vector2(rb.position.x + collisionDistance, rb.position.y), wallSpriteTileLayer)
            || Tile.GetCollider(new Vector2(rb.position.x + collisionDistance, rb.position.y - halfCollisionDistance), wallSpriteTileLayer)
            || Tile.GetCollider(new Vector2(rb.position.x + collisionDistance, rb.position.y + halfCollisionDistance), wallSpriteTileLayer)
            || Physics2D.BoxCast(rb.position, boxCollider.size, 0f, Vector2.right, halfCollisionDistance,
                LayerMasks.ObstacleLayerMask).transform != null)
            && inputX > 0)
        {
            newVelocity.x = 0;
        }
        if ((Tile.GetCollider(new Vector2(rb.position.x - collisionDistance, rb.position.y), wallSpriteTileLayer)
            || Tile.GetCollider(new Vector2(rb.position.x - collisionDistance, rb.position.y - halfCollisionDistance), wallSpriteTileLayer)
            || Tile.GetCollider(new Vector2(rb.position.x - collisionDistance, rb.position.y + halfCollisionDistance), wallSpriteTileLayer)
            || Physics2D.BoxCast(rb.position, boxCollider.size, 0f, Vector2.left, halfCollisionDistance,
                LayerMasks.ObstacleLayerMask).transform != null)
            && inputX < 0)
        {
            newVelocity.x = 0;
        }

        rb.velocity = newVelocity;
    }
}
