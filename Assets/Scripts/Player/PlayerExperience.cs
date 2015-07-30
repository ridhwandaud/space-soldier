using UnityEngine;
using System.Collections;

public class PlayerExperience : MonoBehaviour {

    private int experiencePoints = 0;
    private int experienceToLevelUp = 30;

    public int Level { get; set; }
    public int AvailableSkillPoints { get; set; }

    public void IncrementExperience(int experiencePointsGained)
    {
        experiencePoints += experiencePointsGained;
        print("Gained " + experiencePointsGained + " experience. Now at " + experiencePoints);
        // TODO: Slider stuff.

        if (experiencePoints >= experienceToLevelUp)
        {
            experiencePoints = experiencePoints - experienceToLevelUp;
            LevelUp();
            experienceToLevelUp = calculateExperienceToLevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        AvailableSkillPoints++;
        // TODO: Trigger some other behaviors, such as indicating on the GUI that a new level was achieved.
    }

    private int calculateExperienceToLevelUp()
    {
        //int targetLevel = Level + 1;
        //return targetLevel * 100;
        return 30;
    }
}
