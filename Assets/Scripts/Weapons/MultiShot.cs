using UnityEngine;
using System.Collections.Generic;

public class MultiShot : Weapon
{
    public int energyCost = 0;
    public float degreesBetweenShots;
    [SerializeField]
    private int damage;

    public override float Click()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            int numberOfShots = GetNumberOfShots();
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

    int GetNumberOfShots()
    {
        return Points + 3;
    }

    public override float GetEnergyRequirement()
    {
        return energyCost;
    }

    public override string GetName()
    {
        return "Multishot";
    }

    private void createAndActivateBullet(Vector2 centerDirection, float angleOffset, Transform transform)
    {
        GameObject bullet = StackPool.Pop();
        bullet.transform.position = transform.position;
        Vector2 rotated = VectorUtil.RotateVector(centerDirection, angleOffset);
        float angle = Mathf.Atan2(rotated.y, rotated.x) * Mathf.Rad2Deg;

        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        bullet.SetActive(true);
        bullet.GetComponent<Rigidbody2D>().velocity = VectorUtil.RotateVector(centerDirection, angleOffset) * ProjectileSpeed;
        bullet.GetComponent<BasicPlayerProjectile>().Damage = damage;
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.Damage, damage);
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.NumProjectiles, GetNumberOfShots());
        return dict;
    }

    public override string GetDescription ()
    {
        return "Fires many bullets at once.";
    }
}
