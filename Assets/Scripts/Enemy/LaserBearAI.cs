using UnityEngine;

public class LaserBearAI : EnemyAI {

    private Rigidbody2D rb2d;
    private BasicEnemyFire enemyFireScript;
    private Wander wanderScript;
    private Vector2 colliderSize;
    private Animator animator;

    private float nextActionTime;
    private float walkEndTime;
    private float lastWalkDir = -1;
    private bool charging = false;

    void Awake ()
    {
        nextActionTime = 0;
        rb2d = GetComponent<Rigidbody2D>();
        enemyFireScript = GetComponent<BasicEnemyFire>();
        wanderScript = GetComponent<Wander>();
        colliderSize = GetComponent<BoxCollider2D>().size;
        animator = GetComponent<Animator>();
    }

    void Update () {
        if (KnockbackInProgress || GameSettings.PauseAllEnemies)
        {
            return;
        }

        if (Time.time < nextActionTime)
        {
            return;
        }

        if (EnemyUtil.CanSee(transform.position, Player.PlayerTransform.position))
        {
            if (!animator.GetBool("Charging"))
            {
                rb2d.velocity = Vector2.zero;
                animator.SetBool("Charging", true);
                animator.SetBool("Walking", false);
            }
        }
        else if (!animator.GetBool("Charging"))
        {
            wanderScript.DoWander();
        }

        if (animator.GetBool("Charging"))
        {
            float toPlayer = Player.PlayerTransform.position.x - transform.position.x < 0 ? 0 : 1;
            animator.SetFloat("ToPlayer", toPlayer);
            animator.SetFloat("Facing", toPlayer);
        } else if (rb2d.velocity == Vector2.zero)
        {
            animator.SetBool("Walking", false);
        } else
        {
            animator.SetBool("Walking", true);
            float walkDir = rb2d.velocity.x < 0 ? 0 : 1;
            animator.SetFloat("Facing", walkDir);
            lastWalkDir = walkDir;
        }

    }

    void Fire()
    {
        enemyFireScript.Fire();
        animator.SetBool("Charging", false);
    }
}
