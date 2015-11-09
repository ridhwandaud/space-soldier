using UnityEngine;

public class MultiShot : Weapon
{
    public int energyCost = 0;
    public float degreesBetweenShots;

    public override float Click(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            int numberOfShots = Points + 3;
            bool evenNumberOfShots = numberOfShots % 2 == 0;

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            if (!evenNumberOfShots)
            {
                createAndActivateBullet(direction, 0, transform);
                numberOfShots--;
            }


            for (int i = 0; i < numberOfShots; i++)
            {
                int multiplier = i % 2 == 0 ? 1 : -1;
                float angle = evenNumberOfShots ? (multiplier * degreesBetweenShots / 2) + ((i / 2) * multiplier * degreesBetweenShots * Mathf.Deg2Rad) :
                    (i / 2 + 1) * multiplier * degreesBetweenShots * Mathf.Deg2Rad;

                createAndActivateBullet(direction, angle, transform);
            }

            return energyCost;
        }

        return 0;
    }

    public override float GetEnergyRequirement()
    {
        return energyCost;
    }

    public override string GetName()
    {
        return "multiShot";
    }

    private void createAndActivateBullet(Vector2 centerDirection, float angleOffset, Transform transform)
    {
        GameObject bullet = StackPool.Pop();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.transform.rotation = Quaternion.Euler(VectorUtil.RotateVector(transform.rotation.eulerAngles, angleOffset));
        bullet.SetActive(true);
        bullet.GetComponent<Rigidbody2D>().velocity = VectorUtil.RotateVector(centerDirection, angleOffset) * ProjectileSpeed;
        bullet.GetComponent<BasicPlayerProjectile>().damage = 1;
    }
}
