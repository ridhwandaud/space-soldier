using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour {

    public int Level { get; set; }
    public int AvailableSkillPoints { get; set; }
    public Slider expSlider;
    public Text levelIndicator;

    private int experiencePoints = 0;
    private int experienceToLevelUp = 10;

    void Awake()
    {
        Level = 1;
        expSlider.maxValue = experienceToLevelUp;
    }

    public void IncrementExperience(int experiencePointsGained)
    {
        experiencePoints += experiencePointsGained;

        if (experiencePoints >= experienceToLevelUp)
        {
            experiencePoints = experiencePoints - experienceToLevelUp;
            LevelUp();
        }

        expSlider.value = experiencePoints;
    }

    public void UseSkillPoint()
    {
        AvailableSkillPoints--;
    }

    private void LevelUp()
    {
        Level++;
        AvailableSkillPoints++;
        experienceToLevelUp = calculateExperienceToLevelUp();
        expSlider.maxValue = experienceToLevelUp;

        levelIndicator.text = Level.ToString();

        // TODO: Trigger some other behaviors, such as indicating on the GUI that a new level was achieved.
    }

    private int calculateExperienceToLevelUp()
    {
        //int targetLevel = Level + 1;
        //return targetLevel * 100;
        return 30;
    }
}
