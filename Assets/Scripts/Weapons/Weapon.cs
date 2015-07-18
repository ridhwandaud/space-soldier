using UnityEngine;

public abstract class Weapon
{
    public abstract int GetEnergyCost();

    // Return true if weapon fired successfully.
    public abstract bool Fire(Transform transform);
}