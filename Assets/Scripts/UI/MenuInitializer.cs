using UnityEngine;

public class MenuInitializer : MonoBehaviour {
    public GameObject SkillTree;

	void Update () {
	    if (Input.GetButtonDown("SkillTree"))
        {
            if (Time.timeScale != 0)
            {
                // This naive method of pausing will probably have to be changed later. Having timeScale as 0 isn't going to be good if I ever want my pause menu
                // to have animations or physics. But I don't expect that will happen anytime soon, so I'll leave this minimalist (but functional) solution for now.
                GameState.Paused = true;
                Time.timeScale = 0;
                SkillTree.SetActive(true);
            }
            else {
                GameState.Paused = false;
                Time.timeScale = 1;
                SkillTree.SetActive(false);
            }
        }
	}
}
