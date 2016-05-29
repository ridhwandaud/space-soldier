using UnityEngine;
using System.Collections;

public class CatAI : EnemyAI
{
    private static int GuidedWanderChance = 50;

    [SerializeField]
    private float LaunchDurationMin;
    [SerializeField]
    private float LaunchDurationMax;
    [SerializeField]
    private float CometActivationDistance;
    [SerializeField]
    private float CometContinuationDistance;
    [SerializeField]
    private float CometSpeed;
    [SerializeField]
    private int Damage;
    [SerializeField]
    private int AccuracyVariation;

    private Wander wanderScript;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2D;

    private bool IsComet = false;
    private float CometActivationDistanceSquared;
    private float CometContinuationDistanceSquared;
    private float LaunchStartTime;
    private float LaunchEndTime;

    void Awake () {
        wanderScript = GetComponent<Wander>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        CometActivationDistanceSquared = CometActivationDistance * CometActivationDistance;
        CometContinuationDistanceSquared = CometContinuationDistance * CometContinuationDistance;
    }
	
	void Update () {
        if (IsComet && Time.time > LaunchEndTime)
        {
            if (InCometProximity())
            {
                BeginLaunch();
            } else
            {
                EndLaunch();
            }
        } else if (!IsComet)
        {
            if (InCometProximity())
            {
                BeginLaunch();
            } else
            {
                wanderScript.DoWander(GuidedWanderChance);
            }
        }
    }

    bool InCometProximity()
    {
        return EnemyUtil.CanSee(transform.position, Player.PlayerTransform.position)
            && (transform.position - Player.PlayerTransform.position).sqrMagnitude
            < (IsComet ? CometContinuationDistanceSquared : CometActivationDistanceSquared)
            && EnemyUtil.PathIsNotBlocked(boxCollider2D, transform.position, Player.PlayerTransform.position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (InCometProximity())
            {
                BeginLaunch();
            } else
            {
                EndLaunch();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player.PlayerHealth.InflictDamage(Damage);
        }
    }

    void BeginLaunch()
    {
        spriteRenderer.color = Color.blue;
        Vector2 offset = Player.PlayerTransform.position - transform.position;
        float rotationAmount = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg + Random.Range(-AccuracyVariation, AccuracyVariation);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAmount));
        rb2d.velocity = offset.normalized * CometSpeed;

        IsComet = true;
        LaunchStartTime = Time.time;
        LaunchEndTime = LaunchStartTime + Random.Range(LaunchDurationMin, LaunchDurationMax);
    }

    void EndLaunch()
    {
        IsComet = false;
        spriteRenderer.color = Color.white;
        transform.rotation = Quaternion.identity;
        rb2d.velocity = Vector2.zero;
        wanderScript.ResetWanderTime();
    }
}
