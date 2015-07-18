using UnityEngine;

public class EnergyGun : Weapon
{

    private float firingDelay = .3f;
    private float bulletSpeed = 20;
    private int energyCost = 1;

    private float nextFiringTime;
    private StackPool orbPool;

    public EnergyGun()
    {
        nextFiringTime = 0;
        orbPool = GameObject.Find("EnergyOrbPool").GetComponent<StackPool>();
    }

    public override bool Fire(Transform transform)
    {
        if (Time.time > nextFiringTime)
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

            return true;
        }

        return false;
    }

    public override int GetEnergyCost()
    {
        return energyCost;
    }
}