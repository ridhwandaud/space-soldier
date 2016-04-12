using UnityEngine;
using UnityEngine.UI;

public class MenuInitializer : MonoBehaviour {
    [SerializeField]
    private CanvasGroup skillTreeCanvasGroup;
    [SerializeField]
    private CanvasGroup inventoryCanvasGroup;

    private static bool MenuLocked = false;

	void Update () {
        if (GameState.InputLocked)
        {
            return;
        }

	    if (Input.GetButtonDown("SkillTree") && !MenuLocked)
        {
            if (!GameState.Paused)
            {
                if (GameState.TutorialMode)
                {
                    TutorialEngine.Instance.Trigger(TutorialTrigger.SkillTreeOpened);
                }
                OpenMenu(skillTreeCanvasGroup);
            }
            else if (MenuIsOpen(skillTreeCanvasGroup)) {
                CloseMenu(skillTreeCanvasGroup);
            }
        }

        // TODO: Make a single menu from which both the skill tree and the inventory can be accessed.
        if (Input.GetButtonDown("Inventory") && !MenuLocked)
        {
            if (!GameState.Paused)
            {
                if (GameState.TutorialMode)
                {
                    TutorialEngine.Instance.Trigger(TutorialTrigger.OpenInventory);
                }
                OpenMenu(inventoryCanvasGroup);
            }
            else if (MenuIsOpen(inventoryCanvasGroup))
            {
                if (GameState.TutorialMode)
                {
                    TutorialEngine.Instance.Trigger(TutorialTrigger.CloseInventory);
                }
                CloseMenu(inventoryCanvasGroup);
            }
        }
	}

    void OpenMenu(CanvasGroup menu)
    {
        GameState.PauseGame();
        menu.blocksRaycasts = true;
        menu.alpha = 1;
    }

    void CloseMenu(CanvasGroup menu)
    {
        GameState.UnpauseGame();
        menu.blocksRaycasts = false;
        menu.alpha = 0;
    }

    bool MenuIsOpen(CanvasGroup menu)
    {
        return menu.blocksRaycasts == true && menu.alpha == 1;
    }

    public static void LockMenu()
    {
        MenuLocked = true;
    }

    public static void UnlockMenu()
    {
        MenuLocked = false;
    }
}
