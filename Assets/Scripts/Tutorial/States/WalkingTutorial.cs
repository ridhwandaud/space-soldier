using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkingTutorial : TutorialState
{
    private bool walkingMissionCompleted = false;

    public override void Initialize ()
    {
        Debug.Log("Rendering text");
        RenderText("Move with the W, A, S, and D keys. Try walking around the room a bit.");
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        if (trigger.Equals(TutorialTrigger.Walk) && !walkingMissionCompleted)
        {
            walkingMissionCompleted = true;
            CompleteMission();
        }
    }

    void CompleteMission()
    {
        Invoke("GoToNextState", 3f);
    }
}
