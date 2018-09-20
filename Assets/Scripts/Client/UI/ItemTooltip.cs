using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour {

    public static Dictionary<string, string> Colors = new Dictionary<string, string> {
        {"rouge", "<color=#aa0000ff>" },
        {"vert", "<color=#00cc00ff>" },
        {"bleu", "<color=#0000ccff>" },
        {"noir", "<color=#000000ff>" },
        {"blanc", "<color=#ffffffff>" },
        {"rouge2", "<color=#B7211Eff>" },
        {"marron", "<color=#523828ff>" },
        {"bleu2", "<color=#39C4E2ff>" },
        {"vert2", "<color=#64CF5Dff>" },
        {"rose", "<color=#B71E46ff>" }
    };
    
    private Text tooltip;
    private RectTransform rectTransform; 
    
    void Start () {
        tooltip = GetComponentInChildren<Text>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
	}

    void LateUpdate()
    {
        AdjustPosition();
    }

    public void GenerateTooltip(Item item)
    {
        string statText = "";
        string color;

        if (item.stats.Count > 0)
        {
            statText += "\n\n";
            foreach (var stat in item.stats)
            {
                color = "<color=#" + stat.type.color + ">";
                statText += color + "<b>" + stat.type.name + "</b></color> : ";
                color = stat.minValue >= 0 ? Colors["vert"] : Colors["rouge"];
                if (stat.maxValue > stat.minValue)
                    statText += color + "<b>" + stat.minValue + "</b></color> <size=14>à</size> " + color + "<b>" + stat.maxValue + "</b></color>\n";
                else
                    statText += color + "<b>" + stat.minValue + "</b></color>\n";
            }
            statText = statText.Remove(statText.Length - 1);
        }
        tooltip.text = string.Format("<b><size=18>{0}</size></b>\n<i>{1}</i>{2}", item.title, item.description, statText);
        gameObject.SetActive(true);
    }

    public void AdjustPosition()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

        // float adjustBottom = rectTransform.position.y - rectTransform.sizeDelta.y;
        // float adjustRight = rectTransform.position.x + rectTransform.sizeDelta.x;
        // rectTransform.pivot = new Vector2(adjustRight > Screen.width ? 1.0f : 0.0f, rectTransform.pivot.y);
        // rectTransform.position = new Vector3(transform.position.x + (adjustRight > Screen.width ? -10 : 10), transform.position.y - (adjustBottom < 10 ? adjustBottom : 10), transform.position.z);
    }
}
