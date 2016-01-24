using UnityEngine;
using UnityEngine.UI;

public class MenuInitializer : MonoBehaviour {
    [SerializeField]
    private CanvasGroup skillTreeCanvasGroup;
    [SerializeField]
    private CanvasGroup inventoryCanvasGroup;

	void Update () {
	    if (Input.GetButtonDown("SkillTree"))
        {
            if (!GameState.Paused)
            {
                OpenMenu(skillTreeCanvasGroup);
            }
            else if (MenuIsOpen(skillTreeCanvasGroup)) {

                CloseMenu(skillTreeCanvasGroup);
            }
        }

        // TODO: Make a single menu from which both the skill tree and the inventory can be accessed.
        if (Input.GetButtonDown("Inventory"))
        {
            if (!GameState.Paused)
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.OpenInventory);
                OpenMenu(inventoryCanvasGroup);
            }
            else if (MenuIsOpen(inventoryCanvasGroup))
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.CloseInventory);
                CloseMenu(inventoryCanvasGroup);
            }
        }
	}

    void OpenMenu(CanvasGroup menu)
    {
        GameState.Paused = true;
        Time.timeScale = 0;
        menu.blocksRaycasts = true;
        menu.alpha = 1;
    }

    void CloseMenu(CanvasGroup menu)
    {
        GameState.Paused = false;
        Time.timeScale = 1;
        menu.blocksRaycasts = false;
        menu.alpha = 0;
    }

    bool MenuIsOpen(CanvasGroup menu)
    {
        return menu.blocksRaycasts == true && menu.alpha == 1;
    }
}
