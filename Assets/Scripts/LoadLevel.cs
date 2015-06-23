using UnityEngine;
using System.Collections;
using SpriteTile;

public class LoadLevel : MonoBehaviour {

    public TextAsset level;

	void Awake () {
        Tile.SetCamera();
        Tile.LoadLevel(level);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
