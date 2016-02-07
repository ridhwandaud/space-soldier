using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TutorialState : MonoBehaviour {
    [SerializeField]
    private TutorialState nextState;

    public virtual void Initialize () { }
    public abstract void Trigger (TutorialTrigger trigger);

    protected delegate void TutFunc ();

    private bool awaitingConfirmation = false;

    // These fields are used for tutorial states that have steps that require confirmation.
    private List<TutFunc> blockingActions;
    private int nextBlockingActionIndex = 0;
    private bool pauseForSequence = false;

    public virtual void Update () { }

    protected void RenderText(string textToRender)
    {
        TutorialEngine.Instance.RenderText(textToRender);
    }

    protected void ClearText()
    {
        TutorialEngine.Instance.RenderText("");
    }

    protected IEnumerator ExecuteSequence(List<TimedTutorialAction> tutorialActions)
    {
        for (int x = 0; x < tutorialActions.Count; x++)
        {
            TimedTutorialAction curr = tutorialActions[x];
            curr.Action();
            yield return StartCoroutine(WaitForRealSeconds(curr.Seconds));
        }
    }

    protected void LoadBlockingSteps(List<TutFunc> actions, bool pause = false)
    {
        TutorialEngine.Instance.ShowContinueText();
        awaitingConfirmation = true;
        if (pause)
        {
            GameState.PauseGame();
            pauseForSequence = true;
        }
        blockingActions = actions;
        actions[0]();
        nextBlockingActionIndex = 1;
    }

    protected IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    protected struct TimedTutorialAction
    {
        public TutFunc Action;
        public float Seconds;

        public TimedTutorialAction(TutFunc action, float seconds)
        {
            Action = action;
            Seconds = seconds;
        }
    }

    protected void GoToNextState()
    {
        TutorialEngine.Instance.ChangeState(nextState);
    }

    public void ConfirmInstruction()
    {
        if (awaitingConfirmation && blockingActions.Count > 0)
        {
            blockingActions[nextBlockingActionIndex++]();
            awaitingConfirmation = nextBlockingActionIndex < blockingActions.Count;
            if (!awaitingConfirmation)
            {
                // Must use this variable to avoid wrongly unpausing a game during a menu tutorial sequence.
                if (pauseForSequence)
                {
                    GameState.UnpauseGame();
                }
                pauseForSequence = false;
                TutorialEngine.Instance.HideContinueText();
            }
        }
    }
}