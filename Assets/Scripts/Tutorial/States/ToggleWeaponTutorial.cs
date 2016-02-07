using UnityEngine;
using System.Collections.Generic;

public class ToggleWeaponTutorial : TutorialState
{
    private bool openInventoryMissionAssigned = true;
    private bool equipMissionAssigned = false;
    private bool closeInventoryMissionAssigned = false;
    private bool toggleMissionAssigned = false;

    public override void Initialize ()
    {
        RenderText("Good job. It's time for your final lesson: changing weapons. Go back to your inventory by pressing the Shift key.");
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        switch (trigger)
        {
            case TutorialTrigger.OpenInventory:
                if(openInventoryMissionAssigned)
                {
                    openInventoryMissionAssigned = false;
                    MenuInitializer.LockMenu();
                    RenderText("Notice that each side of the inventory has two slots. Drag the machine gun tile into one of the open " +
                        "left slots.");
                    equipMissionAssigned = true;
                }
                break;
            case TutorialTrigger.SecondLeftWeaponEquipped:
                if (equipMissionAssigned)
                {
                    equipMissionAssigned = false;
                    MenuInitializer.UnlockMenu();
                    RenderText("Both of your weapons are now assigned to the left mouse button. " +
                        "Now exit the inventory with the shift key.");
                    closeInventoryMissionAssigned = true;
                }
                break;
            case TutorialTrigger.CloseInventory:
                if (closeInventoryMissionAssigned)
                {
                    closeInventoryMissionAssigned = false;
                    RenderText("To toggle your left weapon, press the 'q' key. Try toggling a few times. Notice that your weapon " +
                        "changes each time.");
                    toggleMissionAssigned = true;
                }
                break;
            case TutorialTrigger.LeftWeaponSwitched:
                if (toggleMissionAssigned)
                {
                    toggleMissionAssigned = false;
                    Invoke("CompleteMission", 4f);
                }
                break;
        }
    }

    void CompleteMission()
    {
        LoadBlockingSteps(new List<TutFunc>()
        {
            () => RenderText("Good work. You can also toggle your right-hand weapon with the 'e' key, though you won't be " + 
                "able to do that right now since both of your weapons are assigned to your left side."),
            () => RenderText("If you ever forget any of the controls, press the 'Esc' key and navigate to the 'Controls' page."),
            () => RenderText("The goal of the game is to destroy as many enemies and clear as many levels as you can without dying."),
            () => RenderText("Happy Hunting!")
        });
    }
}
