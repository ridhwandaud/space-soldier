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

            MultiShotLogic.FireMultishot(numberOfShots, damage, true, degreesBetweenShots, ProjectileSpeed,
                StackPool, transform, direction);

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
