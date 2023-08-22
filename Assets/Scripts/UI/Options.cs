using DG.Tweening;
using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject blackBackground;
    [SerializeField] private GameObject optionsMenu;

    private void Start()
    {
        DOTween.Init();
    }

    public void OptionsHandler(bool state)
    {
        if (state)
        {
            blackBackground.SetActive(true);
            optionsMenu.transform.DOScale(1f, .35f);
        }

        else
        {
            blackBackground.SetActive(false);
            optionsMenu.transform.DOScale(0f, .25f);
        }
    }
}
