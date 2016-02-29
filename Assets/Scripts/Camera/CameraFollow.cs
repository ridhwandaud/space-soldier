using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    private Rigidbody2D rb;

    private static float DEFAULT_Z = -10;
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        rb = GameObject.Find("Soldier").GetComponent<Rigidbody2D>();
        transform.position = rb.position;
    }

    void Update()
    {
        Camera.main.orthographicSize = Screen.height / (24f * 3f * 2f);
    }

    void FixedUpdate () {
        Vector3 targetPosition = rb.position;
        targetPosition.z = DEFAULT_Z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, dampTime);
    }
}