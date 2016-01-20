using UnityEngine;

public class TutorialEngine : MonoBehaviour {
    private TutorialState currentState;

    void Awake()
    {
        currentState.Initialize();
    }

    void Update()
    {
        currentState.Update();
    }

	public void Trigger(TutorialTrigger trigger)
    {
        currentState.Trigger(trigger);
    }

    public void ChangeState(TutorialState nextState)
    {
        currentState = nextState;
        currentState.Initialize();
    }

    public void RenderText ()
    {
        // display code in the tutorial console
    }
}
