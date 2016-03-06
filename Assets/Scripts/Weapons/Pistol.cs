using UnityEngine;
using System.Collections.Generic;

/* Currently, this is identical to the MachineGun (except it has a slower rate of fire and a different sprite). However,
 * I have decided to make it a different class for now because the machine gun's behavior will change based on the number of
 * skill points. */

public class Pistol : Weapon
{
    public float energyCost;
    [SerializeField]
    private int damage;

    public override float Click(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            GameObject bullet = StackPool.Pop();
            Vector2 addend = VectorUtil.RotateVector(new Vector2(activeProjectileOffset.x, activeProjectileOffset.y), 
                (FacingLeft ? -transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z) * Mathf.Deg2Rad);
            bullet.transform.position = new Vector2(transform.position.x + addend.x, transform.position.y + addend.y);

            // bullet rotation uses the vector from gun to target, not from player to target.
            Vector2 offset = VectorUtil.DirectionToMousePointer(transform);
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            bullet.GetComponent<BasicPlayerProjectile>().Damage = damage;

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * ProjectileSpeed;

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
        return "Laser Pistol";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.Damage, damage);
        return dict;
    }

    public override string GetDescription ()
    {
        return "Shoots lasers.";
    }
}
