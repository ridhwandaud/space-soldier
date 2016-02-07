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
        // Unlock the space bar.
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        switch (trigger)
        {
            case TutorialTrigger.OpenInventory:
                if (!inventoryOpened)
                {
                    inventoryOpened = true;
                    ExplainInventory();
                }
                break;
            case TutorialTrigger.EquipLaserPistol:
                pistolEquipped = true;
                if (inventoryMissionAssigned)
                {
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
        RenderText("Nicely done. You can access your inventory with the Shift bar at any point during gameplay. Now press \"Space\" again to " +
            "close the inventory menu and return to the game.");
    }

    void ExplainInventory()
    {
        StartCoroutine(ExecuteSequence(new List<TimedTutorialAction>()
        {
            new TimedTutorialAction(() => {
                RenderText("This is your inventory. Every skill is represented by a tile. The tiles in the highlighted section are your unequipped skills.");
                // Highlight the section
            }, 5f),

            new TimedTutorialAction(() => {
                RenderText("Skills equipped in any of these three left slots can be used with the left mouse button.");
            }, 5f),

            new TimedTutorialAction(() => {
                RenderText("Notice the unequipped laser pistol skill in your inventory. Drag that tile into " +
                    "one of the left slots to equip it.");
                // Unlock drag
                inventoryMissionAssigned = true;
            }, 3f),

            new TimedTutorialAction(() =>
            {
                if (pistolEquipped)
                {
                    CongratulatePlayer();
                }
            }, 0f)
        }));
    }
}
