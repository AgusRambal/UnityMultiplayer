using DG.Tweening;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem instance;
    public Tooltip Tooltip;

    private void Awake()
    {
        DOTween.Init();
        instance = this;
    }

    public static void Show(string content, string header = "")
    { 
        instance.Tooltip.SetText(content, header);
        instance.Tooltip.transform.DOScale(1f, .1f);
    }

    public static void Hide()
    {
        instance.Tooltip.transform.DOScale(0f, .1f);
    }
}
