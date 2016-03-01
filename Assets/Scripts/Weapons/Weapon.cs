using UnityEngine;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour
{
    public int Points;
    public StackPool StackPool;
    public float FiringDelay;
    public float ProjectileSpeed;
    public Vector2 LocalLeftProjectileOffset;
    public Vector2 LocalRightProjectileOffset;
    public Vector2 LeftOffset;
    public Vector2 RightOffset;

    public bool FacingLeft = true;

    protected float nextFiringTime = 0;
    protected Vector2 activeProjectileOffset;

    public abstract float GetEnergyRequirement();
    public abstract float Click(Transform transform);
    public abstract string GetName();
    public abstract string GetDescription ();
    public abstract Dictionary<string, object> GetProperties ();

    public virtual float Release(Transform transform)
    {
        return 0;
    }

    protected bool CanFire()
    {
        return Time.time > nextFiringTime;
    }

    public void AddPoints()
    {
        Points++;
    }

    public void SetToRightSide()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        transform.localPosition = LeftOffset;
        transform.rotation = Quaternion.Euler(0, 0, VectorUtil.AngleToMousePointer(transform));
        activeProjectileOffset = LocalRightProjectileOffset;
        FacingLeft = false;
    }

    public void SetToLeftSide()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        transform.localPosition = RightOffset;
        transform.rotation = Quaternion.Euler(0, 180, 180 - VectorUtil.AngleToMousePointer(transform));
        activeProjectileOffset = LocalLeftProjectileOffset;
        FacingLeft = true;
    }
}