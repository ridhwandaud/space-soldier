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

    public abstract int GetEnergyRequirement();

    public abstract int Click(Transform transform);

    public abstract string GetName();

    public virtual int Release(Transform transform)
    {
        // No-op.
        return 0;
    }

    protected int GetPoints()
    {
        return skillTree.GetNumPointsForSkill(GetName());
    }

    protected bool CanFire()
    {
        return Time.time > nextFiringTime;
    }
}