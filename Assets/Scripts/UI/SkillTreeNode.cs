using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeNode : MonoBehaviour {
    public int MinLevelRequirement;
    public int MaxPointValue;
    public int Points;
    public bool Unlocked;
    public List<SkillTreeNode> Children;
    public List<SkillDependency> Dependencies;

    private Button button;
    private Text pointsText;
    private PlayerWeaponControl playerWeaponControl;

    void Awake()
    {
        button = GetComponent<Button>();
        pointsText = GetComponent<Text>();
        playerWeaponControl = GetComponent<PlayerWeaponControl>();
    }

    public void OnClick()
    {
        if (Unlocked)
        {
            // Do stuff.
        }
    }

    public void IncrementPoints()
    {
        if (Points < MaxPointValue)
        {
            Points++;
            pointsText.text = Points.ToString();
            // Increment the points in the skill GameObject if it exists, otherwise add into inventory
        }
    }

    void UnlockIfNecessary()
    {
        for (int i = 0; i < Dependencies.Count; i++)
        {
            if(!Dependencies[i].HasEnoughPoints())
            {
                return;
            }
        }

        if (Player.PlayerExperience.Level >= MinLevelRequirement)
        {
            Unlocked = true;
            // Make button not greyed out.
        }
    }

    [System.Serializable]
    public class SkillDependency
    {
        public SkillTreeNode Dependency;
        public int DependencyPointsRequirement;

        public bool HasEnoughPoints()
        {
            return Dependency.Points >= DependencyPointsRequirement;
        }
    }
}
