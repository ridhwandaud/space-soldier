using UnityEngine;
using System.Collections.Generic;

public class EnergyGun : Weapon
{
    public float energyCost;
    [SerializeField]
    private int explosionDamage = 3;
    [SerializeField]
    private float explosionRadius = .6f;
    [SerializeField]
    private int damage;

    public override float Click(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            GameObject orb = StackPool.Pop();
            EnergyOrbProperties properties = orb.GetComponent<EnergyOrbProperties>();
            properties.ExplosionDamage = explosionDamage;
            properties.ExplosionRadius = explosionRadius;
            orb.transform.position = transform.position;
            orb.transform.rotation = transform.rotation;

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            orb.SetActive(true);
            orb.GetComponent<Rigidbody2D>().velocity = direction * ProjectileSpeed;

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
        return "Energy Gun";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.Damage, damage);
        dict.Add(WeaponProperties.ExplosiveDamage, explosionDamage);
        return dict;
    }

    public override string GetDescription ()
    {
        return "Fires a powerful exploding ball of energy.";
    }
}