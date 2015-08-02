using UnityEngine;
using System.Collections.Generic;

public class SkillTree : MonoBehaviour {
    private Dictionary<string, int> skillPointsDictionary = new Dictionary<string, int>()
    {
        {"machineGun", 0},
        {"energyGun", 0},
        {"multiShot", 0}
    };

    public int GetNumPointsForSkill(string skill)
    {
        return skillPointsDictionary[skill];
    }
}
