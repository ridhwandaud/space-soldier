using UnityEngine;
using System.Collections;
using System;

public class FiringTutorial : TutorialState
{
    [SerializeField]
    private Destructible crate;

    public override void Initialize ()
    {
        RenderText("Now that you've equipped your weapon, you can start shooting things. Use your left mouse button to destroy the crate to clear the path into the next room.");
        crate.SetInvincibility(false);
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        switch (trigger)
        {
            case TutorialTrigger.ItemDestroyed:
                RenderText("Nice work. Now head into the next room. Be careful - an enemy is waiting for you.");
                break;
            case TutorialTrigger.CombatTutorialRoomEntered:
                GoToNextState();
                break;
        }
    }
}
