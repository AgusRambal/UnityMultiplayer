using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<Sprite> backgrounds = new List<Sprite>();
    [SerializeField] private Image background;
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private List<GameObject> middleButtons = new List<GameObject>();
    [SerializeField] private List<GameObject> backButtons = new List<GameObject>();//change the name to bottom
    [SerializeField] private GameObject lobbiesPanel;

    void Start()
    {
        DOTween.Init();
        background.sprite = backgrounds[Random.Range(0, backgrounds.Count)];
        SetAnims();
    }

    private void SetAnims()
    {
        for (int i = 0; i < middleButtons.Count; i++) 
        {
            float time = Random.Range(.2f, .9f);

            middleButtons[i].transform.DOScale(1f, time);

            middleButtons[i].transform.DOScale(.75f, time);
        }

        for (int i = 0; i < backButtons.Count; i++)
        {
            float time = Random.Range(.2f, .9f);

            backButtons[i].transform.DOScale(.75f, time);

            backButtons[i].transform.DOScale(.5f, time);

        }
    }

    //Buttons
    public async void StartHost()
    {
        await HostSingleton.Instance.gameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.gameManager.StartClientAsync(joinCodeField.text);
    }

    public void ExitGame()
    { 
        Application.Quit();
    }

    public void InteractWithLobbiesPanel(bool state)
    {
        if (state)
        {
            lobbiesPanel.transform.DOScale(1f, .35f);
        }

        else
        {
            lobbiesPanel.transform.DOScale(0f, .25f);
        }
    }
}
