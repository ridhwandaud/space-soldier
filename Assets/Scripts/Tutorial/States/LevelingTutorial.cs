using UnityEngine;
using System.Collections.Generic;

public class LevelingTutorial : TutorialState
{
    private bool skillTreeMissionAssigned = false;
    private bool clickMissionAssigned = false;

    public override void Initialize()
    {
        // Do nothing.
    }

    public override void Trigger(TutorialTrigger trigger)
    {
        switch(trigger)
        {
            case TutorialTrigger.EnemyKilled:
                LoadBlockingSteps(new List<TutFunc>()
                    {
                        () => RenderText("Congratulations. You've leveled up! You can see your current level in the upper right hand display."),
                        () => {
                            RenderText("Press the Tab key to open up your skill tree.");
                            skillTreeMissionAssigned = true;
                        }
                    });
                break;
            case TutorialTrigger.SkillTreeOpened:
                if (skillTreeMissionAssigned)
                {
                    skillTreeMissionAssigned = false;
                    LoadBlockingSteps(new List<TutFunc>()
                    {
                        () => RenderText("This is your skill tree. Every time you level up, you get one skill point. Look at the upper right hand" +
                            " corner to see how many skill points you have."),
                        () => RenderText("Except for the skills in the first row, each skill requires you to have reached a certain level and to have" +
                            " obtained the pre-requisite skills."),
                        () => RenderText("You can hover over a skill to see its description."),
                        () => {
                            RenderText("Now click on the first non greyed-out skill to acquire it.");
                            clickMissionAssigned = true;
                        }
                    });
                    }
                break;
            case TutorialTrigger.MachineGunAcquired:
                if (clickMissionAssigned)
                {
                    RenderText("Good work! Now you have a machine gun. Press 'Tab' again to close your skill tree and press " +
                        "space to go back to your inventory.");
                    GoToNextState();
                }
                break;
        }
    }
}
