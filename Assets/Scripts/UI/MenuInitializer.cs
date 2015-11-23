using UnityEngine;

public class MenuInitializer : MonoBehaviour {
    public GameObject SkillTree;
    public GameObject Inventory;

	void Update () {
	    if (Input.GetButtonDown("SkillTree"))
        {
            if (!GameState.Paused)
            {
                // This naive method of pausing will probably have to be changed later. Having timeScale as 0 isn't going to be good if I ever want my pause menu
                // to have animations or physics. But I don't expect that will happen anytime soon, so I'll leave this minimalist (but functional) solution for now.
                GameState.Paused = true;
                Time.timeScale = 0;
                SkillTree.SetActive(true);
            }
            else if (SkillTree.activeSelf) {
                GameState.Paused = false;
                Time.timeScale = 1;
                SkillTree.SetActive(false);
            }
        }

        // TODO: Make a single menu from which both the skill tree and the inventory can be accessed.
        if (Input.GetButtonDown("Inventory"))
        {
            if (!GameState.Paused)
            {
                // This naive method of pausing will probably have to be changed later. Having timeScale as 0 isn't going to be good if I ever want my pause menu
                // to have animations or physics. But I don't expect that will happen anytime soon, so I'll leave this minimalist (but functional) solution for now.
                GameState.Paused = true;
                Time.timeScale = 0;
                Inventory.SetActive(true);
            }
            else if (Inventory.activeSelf)
            {
                GameState.Paused = false;
                Time.timeScale = 1;
                Inventory.SetActive(false);
            }
        }
	}
}
