using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TutorialState : MonoBehaviour {
    [SerializeField]
    private TutorialState nextState;

    public abstract void Initialize ();
    public abstract void Trigger (TutorialTrigger trigger);

    protected delegate void TutFunc ();

    public virtual void Update () { }

    protected void RenderText(string textToRender)
    {
        TutorialEngine.Instance.RenderText(textToRender);
    }

    protected IEnumerator ExecuteSequence(List<TutorialAction> tutorialActions)
    {
        for (int x = 0; x < tutorialActions.Count; x++)
        {
            TutorialAction curr = tutorialActions[x];
            curr.Action();
            yield return StartCoroutine(WaitForRealSeconds(curr.Seconds));
        }
    }

    protected IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    protected struct TutorialAction
    {
        public TutFunc Action;
        public float Seconds;

        public TutorialAction(TutFunc action, float seconds)
        {
            Action = action;
            Seconds = seconds;
        }
    }

    protected void GoToNextState()
    {
        TutorialEngine.Instance.ChangeState(nextState);
    }
}