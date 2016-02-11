using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class Tooltip : MonoBehaviour {
    private Text text;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private string separator = ": ";

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

	public void Render(Vector2 position, Weapon weapon, bool showAbove)
    {
        StringBuilder sb = new StringBuilder(weapon.GetName()).AppendLine().AppendLine();
        Dictionary<string, object> properties = weapon.GetProperties();
        foreach (KeyValuePair<string, object> entry in properties)
        {
            sb.Append(entry.Key).Append(separator).Append(entry.Value).AppendLine();
        }

        text.text = sb.AppendLine().Append(weapon.GetDescription()).ToString();
        rectTransform.pivot = showAbove ? new Vector2(.5f, 0) : new Vector2(.5f, 1);
        rectTransform.position = position;
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }
}
