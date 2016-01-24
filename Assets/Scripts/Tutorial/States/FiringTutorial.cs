using UnityEngine;
using System.Collections;
using System;

public class FiringTutorial : TutorialState
{
    private bool blockDestroyed = false;

    public override void Initialize ()
    {
        RenderText("Now that you've equipped your weapon, you can start destroying things. Use your left mouse button to fire the " +
            " laser pistol. Destroy the block in the lower right corner of the room to clear the path to the next room.");
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        switch (trigger)
        {

        }
    }
}
