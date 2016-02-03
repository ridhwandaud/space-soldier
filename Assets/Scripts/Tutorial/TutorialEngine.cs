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
        currentState = initialState;
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

    public void RenderText (string textToRender)
    {
        tutorialConsoleText.text = textToRender;
    }
}
