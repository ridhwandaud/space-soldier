using UnityEngine;

public abstract class TutorialState : MonoBehaviour {

    public abstract void Initialize ();
    public abstract void Update ();
    public abstract void Trigger (TutorialTrigger trigger);
}