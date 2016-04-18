﻿using UnityEngine;
using SpriteTile;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;

    public float speed;
    public float collisionDistance;
    public int wallSpriteTileLayer;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
	}

    void Update()
    {
        if (GameState.Paused || GameState.InputLocked)
        {
            return;
        }

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        float halfCollisionDistance = collisionDistance / 2;
 
        if(GameState.TutorialMode && (inputX != 0 || inputY != 0))
        {
            TutorialEngine.Instance.Trigger(TutorialTrigger.Walk);
        }

        Vector2 newVelocity = new Vector2(inputX, inputY).normalized * speed;

        if ((Tile.GetCollider(new Vector2(rb.position.x, rb.position.y + collisionDistance), BasicLevelDecorator.CliffLayer)
            || Tile.GetCollider(new Vector2(rb.position.x - halfCollisionDistance, rb.position.y + collisionDistance), BasicLevelDecorator.CliffLayer)
            || Tile.GetCollider(new Vector2(rb.position.x + halfCollisionDistance, rb.position.y + collisionDistance), BasicLevelDecorator.CliffLayer)
            || Physics2D.BoxCast(rb.position, boxCollider.size, 0f, Vector2.up, halfCollisionDistance,
                LayerMasks.ObstacleLayerMask).transform != null)
            && inputY > 0)
        {
            newVelocity.y = 0;
        }
        if ((Tile.GetCollider(new Vector2(rb.position.x, rb.position.y - collisionDistance), BasicLevelDecorator.CliffLayer)
            || Tile.GetCollider(new Vector2(rb.position.x - halfCollisionDistance, rb.position.y - collisionDistance), BasicLevelDecorator.CliffLayer)
            || Tile.GetCollider(new Vector2(rb.position.x + halfCollisionDistance, rb.position.y - collisionDistance), BasicLevelDecorator.CliffLayer)
            || Physics2D.BoxCast(rb.position, boxCollider.size, 0f, Vector2.down, halfCollisionDistance,
                LayerMasks.ObstacleLayerMask).transform != null)
            && inputY < 0)
        {
            newVelocity.y = 0;
        }
        if ((Tile.GetCollider(new Vector2(rb.position.x + collisionDistance, rb.position.y), BasicLevelDecorator.CliffLayer)
            || Tile.GetCollider(new Vector2(rb.position.x + collisionDistance, rb.position.y - halfCollisionDistance), BasicLevelDecorator.CliffLayer)
            || Tile.GetCollider(new Vector2(rb.position.x + collisionDistance, rb.position.y + halfCollisionDistance), BasicLevelDecorator.CliffLayer)
            || Physics2D.BoxCast(rb.position, boxCollider.size, 0f, Vector2.right, halfCollisionDistance,
                LayerMasks.ObstacleLayerMask).transform != null)
            && inputX > 0)
        {
            newVelocity.x = 0;
        }
        if ((Tile.GetCollider(new Vector2(rb.position.x - collisionDistance, rb.position.y), BasicLevelDecorator.CliffLayer)
            || Tile.GetCollider(new Vector2(rb.position.x - collisionDistance, rb.position.y - halfCollisionDistance), BasicLevelDecorator.CliffLayer)
            || Tile.GetCollider(new Vector2(rb.position.x - collisionDistance, rb.position.y + halfCollisionDistance), BasicLevelDecorator.CliffLayer)
            || Physics2D.BoxCast(rb.position, boxCollider.size, 0f, Vector2.left, halfCollisionDistance,
                LayerMasks.ObstacleLayerMask).transform != null)
            && inputX < 0)
        {
            newVelocity.x = 0;
        }

        rb.velocity = newVelocity;
        animator.SetInteger("HorizontalAxis", (int)newVelocity.x);
        animator.SetInteger("VerticalAxis", (int)newVelocity.y);
    }
}
