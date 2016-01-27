using UnityEngine;
using UnityEngine.UI;

public class TutorialEngine : MonoBehaviour {
    [SerializeField]
    private TutorialState initialState;
    [SerializeField]
    private Text tutorialConsoleText;

    public static TutorialEngine Instance;

    private TutorialState currentState;

    void Awake ()
    {
        Instance = this;
        if (currentState)
        {
            currentState = initialState;
            currentState.Initialize();
        }
    }

    void Update()
    {
        if (currentState)
        {
            currentState.Update();
        }
    }

	public void Trigger(TutorialTrigger trigger)
    {
        if (currentState)
        {
            currentState.Trigger(trigger);
        }
    }

    public void ChangeState(TutorialState nextState)
    {
        currentState = nextState;
        currentState.Initialize();
    }

    public void RenderText (string textToRender)
    {
        tutorialConsoleText.text = textToRender;
    }
}
