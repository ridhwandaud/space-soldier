using UnityEngine;

public class EnergyGun : Weapon
{
    public int energyCost;

    public override int Click(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + firingDelay;
            GameObject orb = stackPool.Pop();
            orb.transform.position = transform.position;
            orb.transform.rotation = transform.rotation;

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            orb.SetActive(true);
            orb.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

            return energyCost;
        }

        return 0;
    }

    public override int GetEnergyRequirement()
    {
        return energyCost;
    }

    public override string GetName()
    {
        return "machineGun";
    }
}