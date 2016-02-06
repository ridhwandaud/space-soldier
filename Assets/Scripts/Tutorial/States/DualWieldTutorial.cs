using UnityEngine;
using System.Collections.Generic;

public class DualWieldTutorial : TutorialState
{
    private bool firingMissionAssigned = false;

    public override void Initialize()
    {
        // Do nothing. Maybe this shit should be virtual and not abstract.
    }

	public override void Trigger(TutorialTrigger trigger)
    {
        switch(trigger)
        {
            case TutorialTrigger.OpenInventory:
                LoadBlockingSteps(new List<TutFunc>()
                {
                    () => RenderText("Welcome back, mothafucka."),
                    () => RenderText("Notice that yo shizzle is in the right slot now. This means you can fire it by " +
                        "clicking your right mouse button."),
                    () => {
                        RenderText("Close yo shizzle now and try firing, dawg.");
                        firingMissionAssigned = true;
                    }
                });
                break;
        }
    }
}
