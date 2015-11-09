using UnityEngine;

/* Currently, this is identical to the MachineGun (except it has a slower rate of fire and a different sprite). However,
 * I have decided to make it a different class for now because the machine gun's behavior will change based on the number of
 * skill points. */

public class Pistol : Weapon
{
    public float energyCost;

    public override float Click(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            GameObject bullet = StackPool.Pop();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

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
        return "pistol";
    }
}
