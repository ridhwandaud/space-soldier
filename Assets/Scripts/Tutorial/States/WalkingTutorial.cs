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
            Invoke("TeachMouseMovement", 3f);
        }
    }

    void TeachMouseMovement()
    {
        RenderText("Move the mouse to change the direction that you're facing.");
        Invoke("CompleteMission", 3f);
    }

    void CompleteMission()
    {
        RenderText("Nice work.");
        Invoke("GoToNextState", 3f);
    }
}
