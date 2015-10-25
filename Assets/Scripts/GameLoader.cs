using UnityEngine;

public class GameLoader : MonoBehaviour {

    public GameObject gameManager;

	void Awake () {
        if (LoadLevel.instance == null)
        {
            Instantiate(gameManager);
        }
	}
}
