using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    [TextArea] public string content;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(TooltipDelay());
    }

    public IEnumerator TooltipDelay()
    {
        yield return new WaitForSeconds(.2f);

        TooltipSystem.Show(content, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(TooltipDelay());
        TooltipSystem.Hide();
    }
}
