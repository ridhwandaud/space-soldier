using UnityEngine;
using System.Collections;
using SpriteTile;
using System;

public class LoadLevel : MonoBehaviour {

    public TextAsset level;

	void Awake () {
        Tile.SetCamera();
        Tile.LoadLevel(level);

        PolygonCollider2D[] polygonColliders = GameObject.Find("SpriteTileColliders").GetComponentsInChildren<PolygonCollider2D>();
        foreach(PolygonCollider2D collider in polygonColliders) {
            collider.isTrigger = true;
            collider.gameObject.AddComponent(Type.GetType("WallCollision"));
        }
	}
}
