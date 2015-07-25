using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour {

    public GameObject gameManager;

	void Awake () {
        if (LoadLevel.instance == null)
        {
            Instantiate(gameManager);
        }
	}
}
