using UnityEngine;
using System.Collections;

public class BasicEnemyAI : MonoBehaviour {

    public int attackDistance = 7;
    public int chargeDistance = 12;
    public float speed = 2f;
    public float nearbyEnemyRadius = .01f;
    public float chaseTimeSeconds = 3f;

    private GameObject player;
    private Wander wanderScript;
    private BasicEnemyFire fireScript;
    private Rigidbody2D rb2d;

    private int wallLayerMask = 1 << 8; // Layer 8 is the wall layer.
    private int enemyLayerMask = 1 << 9;

    void Awake()
    {
        player = GameObject.Find("Soldier");
        wanderScript = GetComponent<Wander>();
        fireScript = GetComponent<BasicEnemyFire>();
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	void Update () {
        float distanceFromPlayer = Vector3.Distance(player.transform.position, gameObject.transform.position);

        if (CanSeePlayer())
        {
            if (distanceFromPlayer <= attackDistance)
            {
                //rb2d.velocity = Vector2.zero;
                rb2d.velocity = CalculateVelocity(gameObject.transform.position);
                fireScript.Fire();
            }
            else if (distanceFromPlayer <= chargeDistance)
            {
                rb2d.velocity = CalculateVelocity(player.transform.position);
                //rb2d.velocity = (player.transform.position - gameObject.transform.position).normalized * speed;
            }
            else
            {
                rb2d.velocity = CalculateVelocity(gameObject.transform.position);
                //rb2d.velocity = Vector2.zero;
                //rb2d.velocity = (player.transform.position - gameObject.transform.position).normalized * speed;
            }
        }
        else
        {
            rb2d.velocity = CalculateVelocity(gameObject.transform.position);
            //rb2d.velocity = Vector2.zero;
        }
	}

    Vector2 CalculateVelocity(Vector2 target)
    {
        Vector2 pullVector = new Vector2(target.x - gameObject.transform.position.x,
            target.y - gameObject.transform.position.y).normalized * speed;
        Vector2 pushVector = Vector2.zero;

        // Find all nearby enemies
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.transform.position, nearbyEnemyRadius, enemyLayerMask);
        int contenders = 0;

        for (int i = 0; i < nearbyEnemies.Length; i++)
        {
            if (nearbyEnemies[i].transform == gameObject.transform)
            {
                continue;
            }

            Vector2 push = gameObject.transform.position - nearbyEnemies[i].transform.position;
            pushVector += push / push.sqrMagnitude;

            contenders++;
        }

        pullVector *= Mathf.Max(1, contenders);
        pullVector += pushVector;

        return pullVector.normalized * speed;
    }

    bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position, wallLayerMask);
        
        return hit.transform == null;
    }
}
