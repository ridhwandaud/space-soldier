using UnityEngine;
using System.Collections;

public class EnemyMultishot : EnemyWeapon
{
    public float fireInterval;
    public float projectileSpeed;
    public string projectilePoolName;
    public int numberOfShots;
    public float degreesBetweenShots;

    private StackPool projectilePool;
    private float nextFireTime;

    void Start ()
    {
        nextFireTime = Time.time + .5f;
        projectilePool = GameObject.Find(projectilePoolName).GetComponent<StackPool>();
    }

    public override int Fire ()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireInterval;
            MultiShotLogic.FireMultishot(numberOfShots, 0, false, degreesBetweenShots, projectileSpeed, projectilePool,
                transform, Player.PlayerTransform.position - transform.position);
            return 1;
        }

        return 0;
    }
}
