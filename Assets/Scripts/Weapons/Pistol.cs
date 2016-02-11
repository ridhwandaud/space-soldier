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
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
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
