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

    Vector2 RotateVector(Vector2 vector, float radians)
    {
        return new Vector2(vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians), 
            vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians));
    }

    public void DoWander(float speed)
    {
        if (Time.time > nextWanderTime)
        {
            nextWanderTime = Time.time + Random.Range(timeBetweenWanders.x, timeBetweenWanders.y);
            float angle = Random.Range(0f, 2 * Mathf.PI);
            rb.velocity = RotateVector(DEFAULT_ANGLE, angle).normalized * speed;
        }
    }
}
