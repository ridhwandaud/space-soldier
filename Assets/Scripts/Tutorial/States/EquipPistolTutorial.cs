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
        RenderText("Now you'll need a weapon. Press the \"Space\" bar to open up your inventory.");
        // Unlock the space bar.
    }

    public override void Trigger (TutorialTrigger trigger)
    {
        if (trigger.Equals(TutorialTrigger.OpenInventory) && !inventoryOpened)
        {
            inventoryOpened = true;
            ExplainInventory();
        }

        switch (trigger)
        {
            case TutorialTrigger.EquipLaserPistol:
                pistolEquipped = true;
                if (inventoryMissionAssigned)
                {
                    CompleteMission();
                }
                break;
        }
    }

    void CompleteMission()
    {
        RenderText("Nicely done. You can access your inventory with the \"Space\" bar at any point during gameplay. Now press \"Space\" again to " +
            "close the inventory menu and return to the game.");
    }

    void ExplainInventory()
    {
        StartCoroutine(ExecuteSequence(new List<TutorialAction>()
        {
            new TutorialAction(() => {
                RenderText("This is your inventory. Every skill is represented by a tile. The tiles in the highlighted section are your unequipped skills.");
                // Highlight the section
            }, 5f),

            new TutorialAction(() => {
                RenderText("Skills equipped in any of these three left slots can be used with the left mouse button.");
            }, 5f),

            new TutorialAction(() => {
                RenderText("Notice the unequipped laser pistol skill in your inventory. Drag that tile into " +
                    "one of the left slots to equip it.");
                // Unlock drag
                inventoryMissionAssigned = true;
            }, 3f),

            new TutorialAction(() =>
            {
                if (pistolEquipped)
                {
                    CompleteMission();
                }
            }, 0f)
        }));
    }
}
