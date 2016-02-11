using UnityEngine;

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
            bullet.transform.rotation = transform.rotation;
            // Interface for projectiles?
            bullet.GetComponent<BasicPlayerProjectile>().Damage = damage;

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            // this has to be done before setting velocity or it won't work.
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
        return "machineGun";
    }
}
