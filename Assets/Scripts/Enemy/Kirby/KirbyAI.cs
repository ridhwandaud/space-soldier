using UnityEngine;
using System.Collections;

public class KirbyAI : MonoBehaviour {
    public float range;
    public float squaredRange;
    public FiniteStateMachine<KirbyAI> fsm;
    public LineRenderer lineRenderer;

	void Awake () {
        squaredRange = range * range;
        lineRenderer = GetComponent<LineRenderer>();
        fsm = new FiniteStateMachine<KirbyAI>(this, KirbySeekingState.Instance);
	}
	
	void Update () {
        if (!LoadLevel.WALL_COLLIDERS_INITIALIZED)
        {
            return;
        }

        //lineRenderer.enabled = true;
        //lineRenderer.SetPosition(0, transform.position);
        //lineRenderer.SetPosition(1, Player.PlayerTransform.position);
        fsm.Update();
	}
}
