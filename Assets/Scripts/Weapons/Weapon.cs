using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public SkillTree skillTree;
    public StackPool stackPool;
    public float firingDelay;
    public float projectileSpeed;

    protected float nextFiringTime = 0;

    public abstract float GetEnergyRequirement();
    public abstract float Click(Transform transform);
    public abstract string GetName();

    public virtual float Release(Transform transform)
    {
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