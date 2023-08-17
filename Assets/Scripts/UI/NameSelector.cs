using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;

    private const string playerNameKey = "PlayerName";

    private void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        nameField.text = PlayerPrefs.GetString(playerNameKey, string.Empty);
        HandleNameChanged();
    }

    public void HandleNameChanged()
    { 
        connectButton.interactable = nameField.text.Length >= minNameLength && nameField.text.Length <= maxNameLength;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(playerNameKey, nameField.text);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
