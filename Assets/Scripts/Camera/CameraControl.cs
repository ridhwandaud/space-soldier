using UnityEngine;
using System.Collections;

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
        Vector3 targetPosition = cameraEvent == null || cameraEvent.Completed ? rb.position : cameraEvent.Target;
        float activeDampTime = cameraEvent == null ? dampTime : cameraEvent.DampTime;
        targetPosition.z = DEFAULT_Z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, activeDampTime);

        if (cameraEvent != null)
        {
            if (cameraEvent.Completed && ((Vector2)transform.position - rb.position).sqrMagnitude < .5f)
            {
                // Must grab this reference or else the closure will refer to null cameraEvent, causing an exception.
                CameraFunction func = cameraEvent.EndFunction;
                StartCoroutine(Do(cameraEvent.EndWaitTime, func));
                GameState.UnlockInputs();
                cameraEvent = null;
            }
            else if (cameraEvent.Started == false && (cameraEvent.Target - (Vector2)transform.position).sqrMagnitude < 1f)
            {
                StartCoroutine(Do(cameraEvent.FocusHeadWindow, () => cameraEvent.CameraFunction()));
                cameraEvent.Started = true;
            }
        }
    }

    public void LoadCameraEvent(CameraFunction cameraFunction, CameraFunction endFunction, float startWaitTime,
            float endWaitTime, Vector2 target, float dampTime, float focusHeadWindow, float focusTailWindow) {
        StartCoroutine(Do(startWaitTime, () => {
                cameraEvent = new CameraEvent(cameraFunction, endFunction,
                    endWaitTime, target, dampTime, focusHeadWindow, focusTailWindow);
                GameState.LockInputs();
            }
        ));
    }

    public void UnloadCameraEvent()
    {
        StartCoroutine(Do(cameraEvent.FocusTailWindow, () => cameraEvent.Completed = true));
    }

    IEnumerator Do(float waitTime, CameraFunction func)
    {
        yield return new WaitForSeconds(waitTime);
        func();
    }

    // Remember always to unload the camera event when done.
    public class CameraEvent {
        public CameraFunction CameraFunction;
        public CameraFunction EndFunction;
        public Vector2 Target;
        public float EndWaitTime;
        public float FocusHeadWindow;
        public float FocusTailWindow;
        public float DampTime;
        public bool Started;
        public bool Completed;

        public CameraEvent(CameraFunction cameraFunction, CameraFunction endFunction, float endWaitTime, Vector2 target, float dampTime,
            float focusHeadWindow, float focusTailWindow)
        {
            CameraFunction = cameraFunction;
            EndFunction = endFunction;
            Target = target;
            EndWaitTime = endWaitTime;
            Started = false;
            Completed = false;
            DampTime = dampTime;
            FocusHeadWindow = focusHeadWindow;
            FocusTailWindow = focusTailWindow;
        }
    }
}