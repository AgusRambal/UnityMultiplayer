using DG.Tweening;
using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    private void Start()
    {
        DOTween.Init();
    }

    public void OptionsHandler(bool state)
    {
        if (state)
        {
            optionsMenu.transform.DOScale(.75f, .35f);
        }

        else
        {
            optionsMenu.transform.DOScale(0f, .25f);
        }
    }
}
