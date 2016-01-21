using UnityEngine;
using SpriteTile;
using System.Collections;

public class TutorialLoader : MonoBehaviour {
    [SerializeField]
    private TextAsset tutorialLevel;

	void Awake()
    {
        Tile.SetCamera();
        Tile.LoadLevel(tutorialLevel);
    }
}
