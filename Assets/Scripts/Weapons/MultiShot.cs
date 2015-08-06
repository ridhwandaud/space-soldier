using UnityEngine;
using System.Collections;

public class MultiShot : Weapon
{
    private float firingDelay = .5f;
    private float bulletSpeed = 25;
    private int energyCost = 0;
    private float angleBetweenShots = 10 * Mathf.Deg2Rad;

    private StackPool bulletPool;

    public MultiShot(SkillTree skillTree)
        : base(skillTree)
    {
        bulletPool = GameObject.Find("BulletPool").GetComponent<StackPool>();
    }

    public override bool Fire(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + firingDelay;
            int numberOfShots = GetPoints() + 3;
            bool evenNumberOfShots = numberOfShots % 2 == 0;

            Vector3 mouse = Input.mousePosition;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 direction = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y).normalized;

            if (!evenNumberOfShots)
            {
                createAndActivateBullet(direction, 0, transform);
                numberOfShots--;
            }


            for (int i = 0; i < numberOfShots; i++)
            {
                int multiplier = i % 2 == 0 ? 1 : -1;
                float angle = evenNumberOfShots ? (multiplier * angleBetweenShots / 2) + ((i / 2) * multiplier * angleBetweenShots) :
                    (i / 2 + 1) * multiplier * angleBetweenShots;

                createAndActivateBullet(direction, angle, transform);
            }

            return true;
        }

        return false;
    }

    public override int GetEnergyCost()
    {
        return energyCost;
    }

    public override string GetName()
    {
        return "multiShot";
    }

    private void createAndActivateBullet(Vector2 centerDirection, float angleOffset, Transform transform)
    {
        GameObject bullet = bulletPool.Pop();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.transform.rotation = Quaternion.Euler(VectorUtil.RotateVector(transform.rotation.eulerAngles, angleOffset));
        bullet.SetActive(true);
        bullet.GetComponent<Rigidbody2D>().velocity = VectorUtil.RotateVector(centerDirection, angleOffset) * bulletSpeed;
    }
}
