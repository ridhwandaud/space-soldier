using UnityEngine;
using System.Collections.Generic;

public class CombatTutorial : TutorialState
{

    public override void Initialize ()
    {
        LoadBlockingSteps(new List<TutFunc>()
        {
            () => RenderText("Welcome to the combat portion of your training. The most important thing " +
                "to keep track of is the health bar in the upper left hand corner."),
            () => RenderText("Pay close attention to this health bar. Each time you get hit, you will lose " +
                "a little bit of health. When it's all gone, you die."),
            () => RenderText("To defeat an enemy, shoot it until it disappears."),
            () => RenderText("Once you're ready, press the space bar to begin."),
            () => ClearText()
        }, true);
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        if (trigger == TutorialTrigger.EnemyKilled)
        {
            LoadBlockingSteps(new List<TutFunc>()
        {
            () => RenderText("Nice job! You gained some experience points from killing that enemy. This yellow bar in the upper " +
                "right hand corner of the screen shows your experience."),
            () => RenderText("Once your experience meter fills up, you will level up."),
            () => {
                RenderText("You're almost at the next level already! There's one more enemy waiting for you in the next room - destroy " +
                "the block obstructing your path and go defeat it.");
                GoToNextState();
                Invoke("ClearText", 3f);
            }
        });
        }
    }
}
