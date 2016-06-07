using UnityEngine;

public class KnightAI : EnemyAI {
    // 1 Attack = a volley of shots.
    public float timeAfterClosingBeforeBlooming;
    public float timeBeforeClosing;
    public float closeDuration;
    public int numShotsPerVolley;
    public int damage;

    private bool attacking = false;
    private bool readyToAttack = true;
    private float nextAttackTime = 0;
    private int shotsFiredThisTurn = 0;
    private bool closeInvoked = false;

    private EnemyHealth enemyHealth;
    private BasicEnemyFire firingScript;
    private Animator animator;

	void Awake () {
        enemyHealth = GetComponent<EnemyHealth>();
        firingScript = GetComponent<BasicEnemyFire>();
        animator = GetComponent<Animator>();

        ActivateGuard();
	}
	
	// Update is called once per frame
	void Update () {
        if (KnockbackInProgress || GameSettings.PauseAllEnemies || killed)
        {
            return;
        }

        ChaseIfNecessary();

        if (EnemyUtil.CanSee(transform.position, Player.PlayerTransform.position) && !attacking && Time.time > nextAttackTime
            && readyToAttack)
        {
            shotsFiredThisTurn = 0;
            animator.SetTrigger("Bloom");
            readyToAttack = false;
        }

        if (attacking)
        {

            if (shotsFiredThisTurn >= numShotsPerVolley)
            {
                if (!closeInvoked)
                {
                    nextAttackTime = Time.time + timeAfterClosingBeforeBlooming + timeBeforeClosing + closeDuration;
                    Invoke("Close", timeBeforeClosing);
                    closeInvoked = true;
                }
            } else
            {
                shotsFiredThisTurn += firingScript.Fire();
            }
        }
	}

    void Close()
    {
        readyToAttack = true;
        attacking = false;
        enemyHealth.guarded = true;
        animator.SetTrigger("Close");
    }

    void ActivateAttack()
    {
        animator.SetTrigger("Attack");
        animator.SetBool("Hit", false);
        attacking = true;
        closeInvoked = false;
        enemyHealth.guarded = false;
    }

    // TODO: Remove
    void ActivateGuard()
    {
        attacking = false;
        enemyHealth.guarded = true;
    }
}
