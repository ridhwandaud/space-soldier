using UnityEngine;

public abstract class Weapon
{
    private SkillTree skillTree;

    protected Weapon(SkillTree skillTree)
    {
        this.skillTree = skillTree;
    }

    public abstract int GetEnergyCost();

    // Return true if weapon fired successfully.
    public abstract bool Fire(Transform transform);

    public abstract string GetName();

    protected int getPoints()
    {
        return skillTree.GetNumPointsForSkill(GetName());
    }
}