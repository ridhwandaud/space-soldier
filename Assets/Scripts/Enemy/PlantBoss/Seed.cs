using UnityEngine;
using System.Collections;

public class Seed : MonoBehaviour {

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private GameObject plantEnemyPrefab;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        plantEnemyPrefab = Resources.Load("Knight") as GameObject;
    }

	public void Launch(Vector2 initialPosition, Vector2 direction, float initialSeedSpeed)
    {
        transform.position = initialPosition;
        gameObject.SetActive(true);
        rb2d.velocity = direction.normalized * initialSeedSpeed;
    }

    void Grow()
    {
        Instantiate(plantEnemyPrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    void Update()
    {
        float speed = Mathf.Lerp(rb2d.velocity.magnitude, 0, Time.deltaTime * 3);

        if (speed <= 1)
        {
            Grow();
        } else
        {
            rb2d.velocity = rb2d.velocity.normalized * speed;
        }
    }
}
