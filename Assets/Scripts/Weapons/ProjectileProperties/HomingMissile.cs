using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(ProjectileDestroy))]
[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : BasicPlayerProjectile
{
    public float speed;
    public float neighborhoodRadius;

    private int separationDamper = 3; // prevent separation force from being too strong and causing jitters

	void FixedUpdate () {
        Vector3 playerOffset = Player.PlayerTransform.position - transform.position;
        float angle = Mathf.Atan2(playerOffset.y, playerOffset.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        rb2d.velocity = (rb2d.velocity + (Seek() + Separate() * Time.fixedDeltaTime)).normalized * speed;
    }

    Vector2 Seek ()
    {
        return (Vector2)((Player.PlayerTransform.position - transform.position).normalized * speed) - rb2d.velocity;
    }

    Vector2 Separate ()
    {
        Vector3 steeringForce = Vector2.zero;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, neighborhoodRadius, LayerMasks.MissileLayerMask);
        for (int x = 0; x < neighbors.Length; x++)
        {
            Transform neighborTransform = neighbors[x].transform;
            if(neighborTransform != transform)
            {
                steeringForce += (transform.position - neighborTransform.position).normalized / 
                    separationDamper * Vector2.Distance(transform.position, neighborTransform.position);
            }
        }

        return steeringForce;
    }
}
