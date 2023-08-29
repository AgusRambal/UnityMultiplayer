using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text headerField;
    [SerializeField] private TMP_Text contentField;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private int characterWrapLimit;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = "")
    { 
        if (string.IsNullOrEmpty(header)) 
        { 
            headerField.gameObject.SetActive(false);
        }

        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = headerLength > characterWrapLimit || contentLength > characterWrapLimit;
    }

    private void Update()
    {
        Vector2 position = Input.mousePosition;

        //Automatic pivot
        /*float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);*/

        transform.position = position;
    }
}
