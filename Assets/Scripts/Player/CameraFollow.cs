using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    private GameObject player;

    private static float DEFAULT_Z = -10;

    void Awake()
    {
        player = GameObject.Find("Soldier");
    }

	void Update () {
        Vector3 playerPosition = player.transform.position;
        transform.position = new Vector3(playerPosition.x, playerPosition.y, DEFAULT_Z);
	}
}
