using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    private GameObject player;

    private static float DEFAULT_Z = -10;
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        player = GameObject.Find("Soldier");
    }

	void FixedUpdate () {
        Vector3 targetPosition = player.transform.position;
        targetPosition.z = DEFAULT_Z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, dampTime);
    }
}
