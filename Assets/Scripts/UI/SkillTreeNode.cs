using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SkillTreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public int MinLevelRequirement;
    public int MaxPointValue;
    public int Points;
    public bool Unlocked;
    public Tooltip tooltip;
    public Weapon weapon;

    public List<SkillDependency> Dependencies;
    public PlayerWeaponControl playerWeaponControl;

    private Button button;
    private Text pointsText;
    private List<SkillTreeNode> children;
    private RectTransform rectTransform;

    void Awake()
    {
        button = GetComponent<Button>();
        pointsText = GetComponentInChildren<Text>();
        children = new List<SkillTreeNode>();
        rectTransform = GetComponent<RectTransform>();

        Unlocked = Dependencies.Count == 0 && MinLevelRequirement == 1;
    }

    void Start()
    {
        Dependencies.ForEach(dependency => dependency.Dependency.AddChild(this));
    }

    public void OnClick()
    {
        if (TutorialEngine.SkillNodesDisabled)
        {
            return;
        }

        //tooltip.Render(transform.position, weapon);

        if (Unlocked && Player.PlayerExperience.AvailableSkillPoints > 0)
        {
            if (Points == 0)
            {
                //playerWeaponControl.AddWeaponIfAble(weapon);
                InventoryManager.Instance.InstantiateNewTile(new InventoryManager.InventoryTileInfo(null, weapon));
                if (GameState.TutorialMode && weapon.GetName() == "Machine Gun")
                {
                    TutorialEngine.Instance.Trigger(TutorialTrigger.MachineGunAcquired);
                }
            }

            Player.PlayerExperience.UseSkillPoint();
            IncrementPoints();
            children.ForEach(child => child.UnlockIfNecessary());
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

    public void AddChild(SkillTreeNode child)
    {
        children.Add(child);
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

    // fun fact: data.position is in screen space.
    public void OnPointerEnter(PointerEventData data)
    {
        tooltip.Render(rectTransform, weapon);
    }

    public void OnPointerExit(PointerEventData data)
    {
        tooltip.Hide();
    }
}