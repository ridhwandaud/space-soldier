using UnityEngine;

// TODO: Make this inherit from MonoBehaviour and put firingDelay and energyCost on here
// as public properties.
public abstract class Weapon
{
    private SkillTree skillTree;
    protected float nextFiringTime = 0;

    protected Weapon(SkillTree skillTree)
    {
        this.skillTree = skillTree;
    }

    public abstract int GetEnergyCost();

    // Return true if weapon fired successfully.
    // TODO: rename this, since this doesn't actually always fire (it might charge, for instance).
    // Should this return the energy cost, since this can vary?
    public abstract bool Fire(Transform transform);

    public abstract string GetName();

    protected int GetPoints()
    {
        return skillTree.GetNumPointsForSkill(GetName());
    }

    protected bool CanFire()
    {
        return Time.time > nextFiringTime;
    }
}