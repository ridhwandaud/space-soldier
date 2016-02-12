using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	public void LoadScene(int sceneIndex)
    {
        Application.LoadLevel(sceneIndex);
    }
}
