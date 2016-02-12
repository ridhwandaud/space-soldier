using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class Tooltip : MonoBehaviour {
    private Text text;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    [SerializeField]
    private float bufferDistance;

    private string separator = ": ";

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

	public void Render(RectTransform anchor, Weapon weapon)
    {
        bool showAbove = (anchor.position.y / Screen.height) <= .6;
        float tooltipY = showAbove ? anchor.position.y + anchor.sizeDelta.y / 2 + bufferDistance:
            anchor.position.y - (anchor.sizeDelta.y / 2 + bufferDistance);
        Vector2 newPosition = new Vector2(anchor.position.x, tooltipY);


        StringBuilder sb = new StringBuilder(weapon.GetName()).AppendLine().AppendLine();
        Dictionary<string, object> properties = weapon.GetProperties();
        foreach (KeyValuePair<string, object> entry in properties)
        {
            sb.Append(entry.Key).Append(separator).Append(entry.Value).AppendLine();
        }

        text.text = sb.AppendLine().Append(weapon.GetDescription()).ToString();
        rectTransform.pivot = showAbove ? new Vector2(.5f, 0) : new Vector2(.5f, 1);
        rectTransform.position = newPosition;
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }
}
