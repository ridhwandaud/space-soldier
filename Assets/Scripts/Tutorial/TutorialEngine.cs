using UnityEngine;
using UnityEngine.UI;

public class TutorialEngine : MonoBehaviour {
    [SerializeField]
    private TutorialState initialState;
    [SerializeField]
    private Text tutorialConsoleText;
    [SerializeField]
    private float textRatio;

    public static TutorialEngine Instance;

    private TutorialState currentState;
    private float previousScreenWidth;

    void Awake ()
    {
        Instance = this;
        currentState = initialState;
        currentState.Initialize();
        tutorialConsoleText.fontSize = (int)(Screen.width / textRatio);
        previousScreenWidth = Screen.width;
    }

    void Update()
    {
        if (Screen.width != previousScreenWidth)
        {
            tutorialConsoleText.fontSize = (int)(Screen.width / textRatio);
        }

        currentState.Update();

        if (Input.GetButtonDown("ConfirmInstruction"))
        {
            currentState.ConfirmInstruction();
        }
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
