using UnityEngine;
using System.Collections;

public class Wander : MonoBehaviour {

    public Vector2 timeBetweenWanders = new Vector2(1.5f, 1.8f);

    private Rigidbody2D rb;
    private float nextWanderTime;

    private static Vector2 DEFAULT_ANGLE = new Vector2(1, 0);

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        nextWanderTime = Time.time + Random.Range(0f, .3f);
	}

    public void DoWander(float speed)
    {
        if (Time.time > nextWanderTime)
        {
            nextWanderTime = Time.time + Random.Range(timeBetweenWanders.x, timeBetweenWanders.y);
            float angle = Random.Range(0f, 2 * Mathf.PI);
            rb.velocity = VectorUtil.RotateVector(DEFAULT_ANGLE, angle).normalized * speed;
        }
    }
}
