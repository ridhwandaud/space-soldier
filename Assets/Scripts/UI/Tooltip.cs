using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class Tooltip : MonoBehaviour {
    private Text text;
    private CanvasGroup canvasGroup;

    private string separator = ": ";

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

	public void Render(Vector2 position, Weapon weapon)
    {
        StringBuilder sb = new StringBuilder(weapon.GetName()).AppendLine().AppendLine();
        Dictionary<string, object> properties = weapon.GetProperties();
        foreach (KeyValuePair<string, object> entry in properties)
        {
            sb.Append(entry.Key).Append(separator).Append(entry.Value).AppendLine();
        }

        text.text = sb.AppendLine().Append(weapon.GetDescription()).ToString();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
