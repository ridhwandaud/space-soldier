using UnityEngine;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour
{
    public int Points;
    public StackPool StackPool;
    public float FiringDelay;
    public float ProjectileSpeed;

    protected float nextFiringTime = 0;

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
}