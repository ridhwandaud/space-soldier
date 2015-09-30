using UnityEngine;

public class MachineGun : Weapon
{
    public float energyCost;

    public override float Click(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + firingDelay;
            GameObject bullet = stackPool.Pop();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            // Interface for projectiles?
            bullet.GetComponent<BasicPlayerProjectile>().damage = 1;

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            // this has to be done before setting velocity or it won't work.
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

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
        return "machineGun";
    }
}
