using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipPistolTutorial : TutorialState
{
    private bool inventoryOpened = false;
    private bool inventoryMissionAssigned = false;
    private bool pistolEquipped = false;

    public override void Initialize ()
    {
        RenderText("Now you'll need a weapon. Press the Shift key to open up your inventory.");
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        switch (trigger)
        {
            case TutorialTrigger.OpenInventory:
                if (!inventoryOpened)
                {
                    MenuInitializer.LockMenu();
                    inventoryOpened = true;
                    ExplainInventory();
                }
                break;
            case TutorialTrigger.EquipLaserPistol:
                pistolEquipped = true;
                if (inventoryMissionAssigned)
                {
                    MenuInitializer.UnlockMenu();
                    CongratulatePlayer();
                }
                break;
            case TutorialTrigger.CloseInventory:
                if (pistolEquipped)
                {
                    GoToNextState();
                }
                break;
        }
    }

    void CongratulatePlayer()
    {
        RenderText("Nicely done. You can access your inventory with the Shift bar at any point during gameplay. Now press Shift again to " +
            "close the inventory menu and return to the game.");
    }

    void ExplainInventory()
    {
        LoadBlockingSteps(new List<TutFunc>()
        {
            () => RenderText("This is your inventory. Every skill is represented by a tile. The tiles in the highlighted " +
                "section are your unequipped skills."),
            () => RenderText("Skills equipped in any of these three left slots can be used with the left mouse button."),
            () => {
                inventoryMissionAssigned = true;
                RenderText("Notice the unequipped laser pistol skill in your inventory. Drag that tile into " +
                    "one of the left slots to equip it.");
            }
        });
    }
}
