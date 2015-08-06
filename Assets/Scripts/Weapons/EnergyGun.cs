using UnityEngine;

public class EnergyGun : Weapon
{
    private float firingDelay = .3f;
    private float bulletSpeed = 20;
    private int energyCost = 1;

    private StackPool orbPool;

    public EnergyGun(SkillTree skillTree) : base(skillTree)
    {
        orbPool = GameObject.Find("EnergyOrbPool").GetComponent<StackPool>();
    }

    public override int Click(Transform transform)
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + firingDelay;
            GameObject orb = orbPool.Pop();
            orb.transform.position = transform.position;
            orb.transform.rotation = transform.rotation;

            // Fire bullet towards mouse pointer.
            Vector3 mouse = Input.mousePosition;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 direction = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y).normalized;

            // this has to be done before setting velocity or it won't work.
            orb.SetActive(true);

            orb.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

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