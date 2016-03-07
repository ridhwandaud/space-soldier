using UnityEngine;
using System.Collections;

public class Singleton : MonoBehaviour {
    private static GameObject Instance;

	void Awake () {
        if (Instance == null)
        {
            Instance = gameObject;
            if (!GameState.TutorialMode)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            DestroyImmediate(gameObject);
        }
	}
}
