using UnityEngine;
using System.Collections;

public abstract class Skill : MonoBehaviour {

    private SkillTree skillTree;

    public void ActivateSkill(SkillTree skillTree)
    {
        this.skillTree = skillTree;
        gameObject.SetActive(true);
    }

    public abstract string GetName();

    protected int GetPoints()
    {
        return skillTree.GetNumPointsForSkill(GetName());
    }
}
