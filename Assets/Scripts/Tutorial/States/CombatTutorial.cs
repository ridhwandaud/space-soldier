using UnityEngine;
using System.Collections;

public class CombatTutorial : TutorialState
{

    public override void Initialize ()
    {
        RenderText("Welcome to the combat portion of your training. The most important thing to keep track of is the health bar in the upper left hand corner.");
        GameState.PauseGame();
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        
    }
}
