using UnityEngine;

public class CameraControl : MonoBehaviour {
    public delegate void CameraFunction();
    private Rigidbody2D rb;

    private static float DEFAULT_Z = -10;
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    private float lastScreenHeight;
    private CameraEvent cameraEvent;

    void Start()
    {
        rb = GameObject.Find("Soldier").GetComponent<Rigidbody2D>();
        transform.position = rb.position;
    }

    void Update()
    {
        if (Screen.height != lastScreenHeight)
        {
            lastScreenHeight = Screen.height;
            int scalingFactor;
            if (Screen.height < 540)
            {
                scalingFactor = 1;
            } else if (Screen.height < 720)
            {
                scalingFactor = 2;
            } else if (Screen.height < 1080)
            {
                scalingFactor = 3;
            } else
            {
                scalingFactor = 4;
            }
            Camera.main.orthographicSize = Screen.height / (24f * 2f * scalingFactor);
        }
    }

    void FixedUpdate () {
        Vector3 targetPosition = cameraEvent == null ? rb.position : cameraEvent.Target;
        targetPosition.z = DEFAULT_Z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, dampTime);

        if (cameraEvent != null && (cameraEvent.Target - (Vector2)transform.position).sqrMagnitude < 1f)
        {
            cameraEvent.CameraFunction();
        }
    }

    public void LoadCameraEvent(CameraFunction cameraFunction, Vector2 target) {
        cameraEvent = new CameraEvent(cameraFunction, target);
    }

    public void UnloadCameraEvent()
    {
        cameraEvent = null;
    }

    // Remember always to unload the camera event when done.
    public class CameraEvent {
        public CameraFunction CameraFunction;
        public Vector2 Target;

        public CameraEvent(CameraFunction cameraFunction, Vector2 target)
        {
            this.CameraFunction = cameraFunction;
            this.Target = target;
        }
    }
}