using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    [SerializeField]
    private Text levelsBeatenText;
    [SerializeField]
    private Text enemiesSlainText;

	public void PlayAgainButtonClick()
    {
        GameState.ResetGameState();
        Destroy(GameObject.Find("Singletons"));
        Application.LoadLevel(Application.loadedLevel);
    }

    void OnEnable()
    {
        levelsBeatenText.text += GameState.LevelIndex;
        enemiesSlainText.text += GameState.NumEnemiesKilled;
    }
}
