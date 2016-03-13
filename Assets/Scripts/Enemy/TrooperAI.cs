using UnityEngine;

public class TrooperAI : EnemyAI {

    [SerializeField]
    private float timeBetweenActions;
    [SerializeField]
    private float actionVariationTime;
    [SerializeField]
    private float walkTime;
    [SerializeField]
    private float bounceDisplacement;

    private Rigidbody2D rb2d;
    private BasicEnemyFire enemyFireScript;
    private Wander wanderScript;
    private Vector2 colliderSize;

    private float nextActionTime;
    private float walkEndTime;
    private bool walking;

    void Awake ()
    {
        nextActionTime = 0;
        walking = false;
        rb2d = GetComponent<Rigidbody2D>();
        enemyFireScript = GetComponent<BasicEnemyFire>();
        wanderScript = GetComponent<Wander>();
        colliderSize = GetComponent<BoxCollider2D>().size;
    }

    void Update () {
        if (!GameState.WallCollidersInitialized || KnockbackInProgress)
        {
            return;
        }

        if (walking)
        {
            if (Time.time > walkEndTime) {
                walking = false;
                rb2d.velocity = Vector2.zero;
            }
            else
            {
                return;
            }
        }

        if (Time.time < nextActionTime)
        {
            return;
        }

        if (EnemyUtil.CanSee(transform.position, Player.PlayerTransform.position))
        {
            int action = Random.Range(0, 8);
            if (action < 2)
            {
                enemyFireScript.Fire();
            } else if (action < 6)
            {
                rb2d.velocity =
                    EnemyUtil.CalculateUnblockedDirection(transform.position, colliderSize, 2f, false) * speed;
                walkEndTime = Time.time + walkTime + Random.Range(0, actionVariationTime);
                walking = true;
            } else
            {
                rb2d.velocity = Vector2.zero;
            }

            nextActionTime = Time.time + timeBetweenActions + Random.Range(0, actionVariationTime);
        }
        else
        {
            wanderScript.DoWander();
        }
    }

    void OnCollisionEnter2D (Collision2D other)
    {
        if (other.collider.tag == "Wall")
        {
            transform.Translate(-rb2d.velocity.normalized * bounceDisplacement);
            rb2d.velocity = Vector2.zero;
        }
    }
}
