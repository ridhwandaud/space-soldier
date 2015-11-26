using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeTab : MonoBehaviour, IPointerClickHandler {
    // These will eventually be replaced by different frames.
    private static Color ActiveColor = Color.green;
    private static Color InactiveColor = Color.gray;

    [SerializeField]
    private GameObject skillTreeBody;

    // Sadly, these cannot be retrieved programmatically because they start off disabled.
    [SerializeField]
    private GameObject[] otherSkillTreeTabs;
    [SerializeField]
    private GameObject[] otherSkillTreeBodies;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        image.color = skillTreeBody.activeSelf ? ActiveColor : InactiveColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < otherSkillTreeTabs.Length; i++)
        {
            otherSkillTreeTabs[i].GetComponent<Image>().color = InactiveColor;
            otherSkillTreeBodies[i].SetActive(false);
        }
        skillTreeBody.SetActive(true);
        image.color = ActiveColor;
    }
}
