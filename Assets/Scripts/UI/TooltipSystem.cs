using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem instance;
    public Tooltip Tooltip;

    private void Awake()
    {
        instance = this;
    }

    public static void Show(string content, string header = "")
    { 
        instance.Tooltip.SetText(content, header);
        instance.Tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        instance.Tooltip.gameObject.SetActive(false);
    }
}
