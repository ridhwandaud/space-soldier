using UnityEngine;
using System.Collections;

public class MultiShotLogic : MonoBehaviour {

	public static void FireMultishot(int numberOfShots, int damage, bool firedByPlayer, float degreesBetweenShots, float projectileSpeed,
        StackPool stackPool, Transform originTransform, Vector2 direction)
    {
        bool evenNumberOfShots = numberOfShots % 2 == 0;

        if (!evenNumberOfShots)
        {
            createAndActivateBullet(direction, 0, originTransform, stackPool, damage, projectileSpeed, firedByPlayer);
            numberOfShots--;
        }


        for (int i = 0; i < numberOfShots; i++)
        {
            int multiplier = i % 2 == 0 ? 1 : -1;
            float angle = evenNumberOfShots ? (multiplier * degreesBetweenShots / 2) + ((i / 2) * multiplier * degreesBetweenShots * Mathf.Deg2Rad) :
                (i / 2 + 1) * multiplier * degreesBetweenShots * Mathf.Deg2Rad;

            createAndActivateBullet(direction, angle, originTransform, stackPool, damage, projectileSpeed, firedByPlayer);
        }

    }

    private static void createAndActivateBullet (Vector2 centerDirection, float angleOffset, Transform transform, StackPool stackPool,
        int damage, float projectileSpeed, bool firedByPlayer)
    {
        GameObject bullet = stackPool.Pop();
        bullet.transform.position = transform.position;
        Vector2 rotated = VectorUtil.RotateVector(centerDirection, angleOffset);
        float angle = Mathf.Atan2(rotated.y, rotated.x) * Mathf.Rad2Deg;

        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        bullet.SetActive(true);
        bullet.GetComponent<Rigidbody2D>().velocity = VectorUtil.RotateVector(centerDirection, angleOffset).normalized * projectileSpeed;

        if (firedByPlayer)
        {
            bullet.GetComponent<BasicPlayerProjectile>().Damage = damage;
        }
    }
}
