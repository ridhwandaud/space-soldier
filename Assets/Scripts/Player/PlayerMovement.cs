using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D rigidBody2D;

    public float speed = 8;

	void Start () {
        rigidBody2D = GetComponent<Rigidbody2D>();
	}

    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        rigidBody2D.velocity = new Vector2(inputX, inputY).normalized * speed;
    }
}
