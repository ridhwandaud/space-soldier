using UnityEngine;
using System.Collections.Generic;

public class LevelingTutorial : TutorialState
{
    private bool skillTreeMissionAssigned = false;
    private bool clickMissionAssigned = false;

    public override void Trigger(TutorialTrigger trigger)
    {
        switch(trigger)
        {
            case TutorialTrigger.EnemyKilled:
                LoadBlockingSteps(new List<TutFunc>()
                    {
                        () => RenderText("Congratulations. You've leveled up! You can see your current level in the upper right hand display."),
                        () => {
                            RenderText("Press the tab key to open up your skill tree.");
                            skillTreeMissionAssigned = true;
                        }
                    });
                break;
            case TutorialTrigger.SkillTreeOpened:
                if (skillTreeMissionAssigned)
                {
                    MenuInitializer.LockMenu();
                    skillTreeMissionAssigned = false;
                    LoadBlockingSteps(new List<TutFunc>()
                    {
                        () => RenderText("This is your skill tree. Every time you level up, you get one skill point. You can use your skill points to buy new skills."),
                        () => RenderText("Most skills require you to reach a certain level and to have other skills before you can acquire them."),
                        () => RenderText("The greyed-out skills are the ones that you haven't fulfilled the requirements for yet (and therefore cannot purchase)."),
                        () => RenderText("You can hover over a skill to see its description and requirements."),
                        () => {
                            RenderText("Click on the non greyed-out skill in the upper left corner to acquire it.");
                            TutorialEngine.SkillNodesDisabled = false;
                            clickMissionAssigned = true;
                        }
                    });
                    }
                break;
            case TutorialTrigger.MachineGunAcquired:
                if (clickMissionAssigned)
                {
                    MenuInitializer.UnlockMenu();
                    GoToNextState();
                }
                break;
        }
    }
}
