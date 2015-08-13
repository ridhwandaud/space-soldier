using UnityEngine;
using System.Collections;

public class FootSoldierAI : MonoBehaviour {

    public int firingDistance = 10;
    public int attackingDistance = 20;
    public float speed = 2f;
    public float timeBetweenMoves;
    // The angle (in degrees) in each direction that the enemy can move.
    public int movementVariationDegrees;
    public int shotsFiredPerMovement = 5;

    private GameObject player;
    private Rigidbody2D rb2d;
    private float nextMoveTime = 0;
    private int shotsFiredThisMovement = 0;
    private BasicEnemyFire enemyFireScript;

    void Awake()
    {
        player = GameObject.Find("Soldier");
        rb2d = GetComponent<Rigidbody2D>();
        enemyFireScript = GetComponent<BasicEnemyFire>();
    }

    void Update()
    {
        if (WithinAttackRange() && Time.time >= nextMoveTime)
        {
            if (shotsFiredThisMovement < shotsFiredPerMovement)
            {
                rb2d.velocity = Vector2.zero;
                shotsFiredThisMovement += enemyFireScript.Fire();
            }
            else
            {
                shotsFiredThisMovement = 0;
                nextMoveTime = Time.time + timeBetweenMoves;
                int rotation = Random.Range(-movementVariationDegrees, movementVariationDegrees);
                rb2d.velocity = VectorUtil.RotateVector(player.transform.position - gameObject.transform.position, rotation).normalized * speed;
            }
        }
        else
        {
            shotsFiredThisMovement = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Wall")
        {
            rb2d.velocity = Vector2.zero;
        }
    }

    bool WithinAttackRange()
    {
        return Vector3.Distance(player.transform.position, gameObject.transform.position) <= attackingDistance;
    }
}
