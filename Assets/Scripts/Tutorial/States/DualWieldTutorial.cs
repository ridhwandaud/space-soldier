using UnityEngine;
using System.Collections.Generic;

public class DualWieldTutorial : TutorialState
{
    private bool openInventoryMissionAssigned = true;
    private bool rightGunFiringMissionAssigned = false;
    private bool doubleFiringMissionAssigned = false;

    public override void Initialize ()
    {
        RenderText("Good work! Now you have a machine gun. Press Tab again to close your skill tree and press " +
            "Shift to go back to your inventory.");
    }

    public override void Trigger(TutorialTrigger trigger)
    {
        switch (trigger)
        {
            case TutorialTrigger.OpenInventory:
                if (openInventoryMissionAssigned)
                {
                    openInventoryMissionAssigned = false;
                    LoadBlockingSteps(new List<TutFunc>()
                    {
                        () => RenderText("Notice that your machine gun is in the right slot now. This means you can fire it by " +
                            "clicking your right mouse button."),
                        () => {
                            RenderText("Exit the inventory with the shift key and try firing your new weapon with the right mouse button.");
                            rightGunFiringMissionAssigned = true;
                        }
                    });
                }
                break;
            case TutorialTrigger.MachineGunFired:
                if (rightGunFiringMissionAssigned)
                {
                    rightGunFiringMissionAssigned = false;
                    LoadBlockingSteps(new List<TutFunc>()
                    {
                        () => RenderText("Nice work. Notice the blue energy bar in the upper left corner. Whenever you use any weapon or skill other " +
                        "than the laser pistol (the default starting weapon), it will use up some energy. "),
                        () => RenderText("When you run out of energy, you'll only be able to use your pistol."),
                        () => RenderText("Energy recharges slowly over time."),
                        () => {
                            RenderText("Now that you have a left and a right weapon, you can fire both at the same time. Try holding down " +
                            "your left and right mouse buttons at the same time to fire both guns simultaneously.");
                            doubleFiringMissionAssigned = true;
                        }
                });
                }
                break;
            case TutorialTrigger.BothGunsFired:
                if (doubleFiringMissionAssigned)
                {
                    doubleFiringMissionAssigned = false;
                    GoToNextState();
                }
                break;
        }
    }
}
