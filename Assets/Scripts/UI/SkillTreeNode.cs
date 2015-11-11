using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeNode : MonoBehaviour {
    public int MinLevelRequirement;
    public int MaxPointValue;
    public int Points;
    public bool Unlocked;
    public Weapon weapon;
    // Is there a point in explicitly setting the children here? This info is already implicit in the children's Dependencies field. Maybe in Awake, each node
    // can just add itself to the child list of all dependencies. Might save me some manual effort...
    public List<SkillTreeNode> Children;
    public List<SkillDependency> Dependencies;
    public PlayerWeaponControl playerWeaponControl;

    private Button button;
    private Text pointsText;

    void Awake()
    {
        button = GetComponent<Button>();
        pointsText = GetComponentInChildren<Text>();

        Unlocked = Dependencies.Count == 0 && MinLevelRequirement == 1;
    }

    public void OnClick()
    {
        if (Unlocked && Player.PlayerExperience.AvailableSkillPoints > 0)
        {
            if (Points == 0)
            {
                playerWeaponControl.AddWeapon(weapon, PlayerWeaponControl.WeaponSide.Right);
            }

            Player.PlayerExperience.UseSkillPoint();
            IncrementPoints();
            Children.ForEach(child => child.UnlockIfNecessary());
        } else
        {
            print("NA DAWG.");
        }
    }

    public void IncrementPoints()
    {
        if (Points < MaxPointValue)
        {
            Points++;
            pointsText.text = Points.ToString();
            // Increment the points in the skill GameObject (TODO: Add a method for incrementing points in the weapon class).
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
