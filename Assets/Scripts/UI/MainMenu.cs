using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<Sprite> backgrounds = new List<Sprite>();
    [SerializeField] private Image background;

    void Start()
    {
        background.sprite = backgrounds[Random.Range(0, backgrounds.Count)];
    }
}
