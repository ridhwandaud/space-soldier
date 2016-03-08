using UnityEngine;
using System.Collections.Generic;

public class MachineGun : Weapon
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
            FireStandardProjectile(bullet);

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
        return "Machine Gun";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        return dict;
    }

    public override string GetDescription ()
    {
        return "Fires lots of bullets really quickly.";
    }
}
