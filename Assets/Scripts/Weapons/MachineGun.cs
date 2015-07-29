using UnityEngine;

public class MachineGun : Weapon
{

    private float firingDelay = .1f;
    private float bulletSpeed = 50;
    private int energyCost = 0;

    private float nextFiringTime;
    private StackPool bulletPool;

    public MachineGun(SkillTree skillTree) : base(skillTree)
    {
        nextFiringTime = 0;
        bulletPool = GameObject.Find("BulletPool").GetComponent<StackPool>();
    }

    public override bool Fire(Transform transform)
    {
        if (Time.time > nextFiringTime)
        {
            nextFiringTime = Time.time + firingDelay;
            GameObject bullet = bulletPool.Pop();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            // Fire bullet towards mouse pointer.
            Vector3 mouse = Input.mousePosition;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 direction = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y).normalized;

            // this has to be done before setting velocity or it won't work.
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

            return true;
        }

        return false;
    }

    public override int GetEnergyCost()
    {
        return energyCost;
    }

    public override string GetName()
    {
        return "machineGun";
    }
}
