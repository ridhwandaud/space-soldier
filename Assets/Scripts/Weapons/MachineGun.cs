using UnityEngine;

public class MachineGun : Weapon
{
    public int energyCost = 0;

    public override int Click(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + firingDelay;
            GameObject bullet = stackPool.Pop();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            // this has to be done before setting velocity or it won't work.
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

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
