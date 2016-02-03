using UnityEngine;

public class TriggerRegion : MonoBehaviour {
    [SerializeField]
    private TutorialTrigger tutorialTrigger;

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            TutorialEngine.Instance.Trigger(tutorialTrigger);
        }
    }
}
